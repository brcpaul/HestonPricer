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
        // private double q;          // Dividend yield
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
        // private bool antithetic;   // Use antithetic variates for variance reduction

        private Random random;

        public HestonAsianPricer(double r, double q, double S0, double K, double T,
                                double kappa, double theta, double V0, double sigma,
                                double rho, int nbPaths = 100000, int nbSteps = 200,
                                bool antithetic = true)
        {
            this.r = r;
            // this.q = q;
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
            // this.antithetic = antithetic;

            this.random = new Random(); 
        }

        // Generate two correlated normal random variables using Box-Muller and Cholesky decomposition
        private (double, double) GenerateCorrelatedNormals()
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

        // Price an Asian option (arithmetic average)
        public double PriceAsianOption(OptionType optionType = OptionType.Call)
        {
            bool isCall = optionType == OptionType.Call;
            double dt = T / nbSteps;
            double sumPayoffs = 0.0;

            for (int i = 0; i < this.nbPaths; i++)
            {
                double pathPayoff = 0;

                double St = S0;
                double Vt = V0;
                double sumPrices = 0.0;

                for (int step = 0; step < nbSteps; step++)
                {
                    // Generate correlated random variables
                    var (dW1, dW2) = GenerateCorrelatedNormals();

                    // Ensure variance remains positive
                    Vt = Math.Max(0, Vt);
                    double sqrtVt = Math.Sqrt(Vt);

                    Vt += kappa * (theta - Vt) * dt + sigma * sqrtVt * dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                    St *= Math.Exp((r - 0.5 * Vt) * dt + sqrtVt * dW1 * Math.Sqrt(dt)); 

                    // Accumulate the price for the Asian average
                    sumPrices += St;
                }

                // Average price over the period
                double avgPrice = St;//sumPrices / nbSteps;

                // Calculate the payoff
                double payoff = isCall ?
                    Math.Max(0, avgPrice - K) :
                    Math.Max(0, K - avgPrice);

                pathPayoff += payoff;

                sumPayoffs += pathPayoff;
            }

            // Discount the average payoff
            double price = Math.Exp(-r * T) * (sumPayoffs / nbPaths);
            return price;
        }
    }
}