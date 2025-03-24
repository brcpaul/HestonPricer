using HestonPricer.Models;

public class MonteCarloPricer : PricerBase
{
    private HestonParameters hestonParameters;
    private int nbPaths;       // Number of Monte Carlo paths
    private int nbSteps;       // Number of time steps for discretization
                               // private bool antithetic;   // Use antithetic variates for variance reduction

    private Random random;

    public MonteCarloPricer(OptionBase option, HestonParameters hestonParameters, int nbPaths = 100000, int nbSteps = 200): base(option)
    {
        this.hestonParameters = hestonParameters;
        this.nbPaths = nbPaths;
        this.nbSteps = nbSteps;

        random = new Random();
    }

    // Price an Asian option (arithmetic average)
    public override double Price()
    {
        bool isCall = option.IsCall;
        double T = option.Maturity;
        double S0 = option.SpotPrice;
        double K = option.Strike;
        double r = option.RiskFreeRate;
        double kappa = hestonParameters.Kappa;
        double theta = hestonParameters.Theta;
        double V0 = hestonParameters.V0;
        double sigma = hestonParameters.Sigma;
        double rho = hestonParameters.Rho;
        double dt = T / nbSteps;
        double sumPayoffs = 0.0;

        for (int i = 0; i < nbPaths / 2; i++)
        {

            double[] St = new double[] { S0, S0 };
            double[] Vt = new double[] { V0, V0 };
            double[] path = new double[nbSteps];
            double[] pathAnti = new double[nbSteps];

            for (int step = 0; step < nbSteps; step++)
            {
                // Generate correlated random variables
                double[] normals = RandomNumberGenerator.GenerateCorrelatedNormals(rho, threadRandom);
                double dW1 = normals[0];
                double dW2 = normals[1];

                // Ensure variance remains positive
                Vt[0] = Math.Max(0, Vt[0]);
                Vt[1] = Math.Max(0, Vt[1]);

                Vt[0] += kappa * (theta - Vt[0]) * dt + sigma * Math.Sqrt(Vt[0]) * dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                St[0] *= Math.Exp((r - 0.5 * Vt[0]) * dt + Math.Sqrt(Vt[0]) * dW1 * Math.Sqrt(dt));

                if (double.IsNaN(St[0]))
                {
                    throw new Exception("St[0] is NaN");
                }

                Vt[1] += kappa * (theta - Vt[1]) * dt + sigma * Math.Sqrt(Vt[1]) * -dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                St[1] *= Math.Exp((r - 0.5 * Vt[1]) * dt + Math.Sqrt(Vt[1]) * -dW1 * Math.Sqrt(dt));

                path[step] = St[0];
                pathAnti[step] = St[1];
            }

            sumPayoffs += option.Payoff(path) + option.Payoff(pathAnti) * 1;

            if (double.IsNaN(sumPayoffs))
            {
                throw new Exception("Payoff is NaN");
            }
        }

        // Discount the average payoff
        double price = Math.Exp(-r * T) * (sumPayoffs / nbPaths);

        if (price is double.NaN)
        {
            throw new Exception("Price is NaN");
        }

        return price;
    }

    public double[] Price(int nbPrices)
    {

        double[] prices = new double[nbPrices];
        // Parallel.For(0, nbPrices, i =>
        // {
        //     // Generate a new random seed for each thread
        //     Random threadRandom = new Random(i);
        //     prices[i] = PriceHeston(threadRandom);
        // });

        for (int i = 0; i < nbPrices; i++)
        {
            prices[i] = PriceHeston();
        }

        double price = Stats.Mean(prices);
        double stdDev = Stats.StandardDeviation(prices);
        double marginOfError = 1.96 * stdDev / Math.Sqrt(nbPrices);

        return new double[] { price, marginOfError, stdDev };
    }
}