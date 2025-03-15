using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HestonPricer
{
    public enum OptionType
    {
        Call,
        Put
    }

    public class HestonAsianPricer
    {
        private double r;          // Risk-free rate
        private double q;          // Dividend yield
        private double S0;         // Initial spot price
        private double K;          // Strike price
        private double T;          // Maturity
        private double kappa;      // Mean reversion rate
        private double theta;      // Long-term variance
        private double V0;         // Initial variance
        private double sigma;      // Volatility of volatility
        private double rho;        // Correlation between Brownian motions
        private int nbPaths;       // Number of Monte Carlo paths
        private int nbSteps;       // Number of time steps for discretization
        private bool antithetic;   // Use antithetic variates for variance reduction

        // RNG with thread-safe implementation
        private Random random;

        public HestonAsianPricer(double r, double q, double S0, double K, double T,
                                double kappa, double theta, double V0, double sigma,
                                double rho, int nbPaths = 10000, int nbSteps = 20,
                                bool antithetic = true)
        {
            this.r = r;
            this.q = q;
            this.S0 = S0;
            this.K = K;
            this.T = T;
            this.kappa = kappa;
            this.theta = theta;
            this.V0 = V0;
            this.sigma = sigma;
            this.rho = rho;
            this.nbPaths = nbPaths;
            this.nbSteps = nbSteps;
            this.antithetic = antithetic;

            this.random = new Random(42); // Fixed seed for reproducibility
        }

        // Generate two correlated normal random variables using Box-Muller and Cholesky decomposition
        private (double, double) GenerateCorrelatedNormals()
        {
            lock (random) // Thread-safe access to the random generator
            {
                // Box-Muller for generating two independent normals
                double u1 = 1.0 - random.NextDouble();
                double u2 = 1.0 - random.NextDouble();

                double z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
                double z2 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                // Correlation using Cholesky decomposition
                double w1 = z1;
                double w2 = rho * z1 + Math.Sqrt(1 - rho * rho) * z2;

                return (w1, w2);
            }
        }

        // Price an Asian option (arithmetic average)
        public double PriceAsianOption(OptionType optionType = OptionType.Call)
        {
            bool isCall = optionType == OptionType.Call;
            double dt = T / nbSteps;
            double sumPayoffs = 0.0;
            object lockObj = new object(); // Object for thread-safe operations

            // Parallel computation for faster processing
            Parallel.For(0, nbPaths, i =>
            {
                int pathsToSimulate = antithetic ? 2 : 1;
                double pathPayoff = 0;

                for (int pathIdx = 0; pathIdx < pathsToSimulate; pathIdx++)
                {
                    bool isAntitheticPath = pathIdx == 1;

                    double St = S0;
                    double Vt = V0;
                    double sumPrices = 0.0;

                    for (int step = 0; step < nbSteps; step++)
                    {
                        // Generate correlated random variables
                        var (dW1, dW2) = GenerateCorrelatedNormals();

                        if (isAntitheticPath)
                        {
                            // For antithetic paths, negate the random variables
                            dW1 = -dW1;
                            dW2 = -dW2;
                        }

                        // Ensure variance remains positive
                        Vt = Math.Max(0, Vt);
                        double sqrtVt = Math.Sqrt(Vt);

                        // Milstein discretization for the price
                        double dSt = (r - q) * St * dt + sqrtVt * St * dW1 * Math.Sqrt(dt);
                        // Milstein correction term for S
                        dSt += 0.5 * Vt * St * (dW1 * dW1 - 1) * dt;
                        St += dSt;

                        // Milstein discretization for the variance
                        double dVt = kappa * (theta - Vt) * dt + sigma * sqrtVt * dW2 * Math.Sqrt(dt);
                        // Milstein correction term for V
                        dVt += 0.25 * sigma * sigma * (dW2 * dW2 - 1) * dt;
                        Vt += dVt;

                        // Accumulate the price for the Asian average
                        sumPrices += St;
                    }

                    // Average price over the period
                    double avgPrice = sumPrices / nbSteps;

                    // Calculate the payoff
                    double payoff = isCall ?
                        Math.Max(0, avgPrice - K) :
                        Math.Max(0, K - avgPrice);

                    pathPayoff += payoff;
                }

                // Average the payoffs from the original and antithetic paths
                pathPayoff /= pathsToSimulate;

                // Add to total in a thread-safe manner
                lock (lockObj)
                {
                    sumPayoffs += pathPayoff;
                }
            });

            // Discount the average payoff
            double price = Math.Exp(-r * T) * (sumPayoffs / nbPaths);
            return price;
        }

        // Method to calculate price and its confidence interval
        public (double Price, double Error, double LowerBound, double UpperBound) PriceWithConfidenceInterval(
            OptionType optionType = OptionType.Call,
            double confidenceLevel = 0.95)
        {
            bool isCall = optionType == OptionType.Call;
            double dt = T / nbSteps;
            List<double> payoffs = new List<double>(nbPaths);

            Parallel.For(0, nbPaths, i =>
            {
                int pathsToSimulate = antithetic ? 2 : 1;
                double pathPayoff = 0;

                for (int pathIdx = 0; pathIdx < pathsToSimulate; pathIdx++)
                {
                    bool isAntitheticPath = pathIdx == 1;

                    double St = S0;
                    double Vt = V0;
                    double sumPrices = 0.0;

                    for (int step = 0; step < nbSteps; step++)
                    {
                        var (dW1, dW2) = GenerateCorrelatedNormals();

                        if (isAntitheticPath)
                        {
                            dW1 = -dW1;
                            dW2 = -dW2;
                        }

                        // Ensure variance remains positive
                        Vt = Math.Max(0, Vt);
                        double sqrtVt = Math.Sqrt(Vt);

                        // Milstein discretization
                        double dSt = (r - q) * St * dt + sqrtVt * St * dW1 * Math.Sqrt(dt);
                        dSt += 0.5 * Vt * St * (dW1 * dW1 - 1) * dt;
                        St += dSt;

                        double dVt = kappa * (theta - Vt) * dt + sigma * sqrtVt * dW2 * Math.Sqrt(dt);
                        dVt += 0.25 * sigma * sigma * (dW2 * dW2 - 1) * dt;
                        Vt += dVt;

                        sumPrices += St;
                    }

                    double avgPrice = sumPrices / nbSteps;
                    double payoff = isCall ?
                        Math.Max(0, avgPrice - K) :
                        Math.Max(0, K - avgPrice);

                    pathPayoff += payoff;
                }

                pathPayoff /= pathsToSimulate;

                lock (payoffs)
                {
                    payoffs.Add(pathPayoff);
                }
            });

            // Calculate mean payoff
            double meanPayoff = payoffs.Average();

            // Calculate standard deviation
            double variance = payoffs.Sum(x => Math.Pow(x - meanPayoff, 2)) / (payoffs.Count - 1);
            double stdDev = Math.Sqrt(variance);

            // Standard error
            double stdError = stdDev / Math.Sqrt(payoffs.Count);

            // Critical value for the confidence interval
            double z = 1.96; // For 95% confidence
            if (confidenceLevel == 0.99) z = 2.576;
            if (confidenceLevel == 0.90) z = 1.645;

            double error = z * stdError;
            double price = Math.Exp(-r * T) * meanPayoff;
            double lowerBound = price - error;
            double upperBound = price + error;

            return (price, error, lowerBound, upperBound);
        }
    }
}