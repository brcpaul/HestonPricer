using HestonPricer.Models;

public class MonteCarloPricer : PricerBase
{
    private HestonParameters hestonParameters;
    private int nbPaths;       // Number of Monte Carlo paths
    private int nbSteps;       // Number of time steps for discretization
                               // private bool antithetic;   // Use antithetic variates for variance reduction

    private Random random;

    public MonteCarloPricer(OptionBase option, HestonParameters hestonParameters, int nbPaths = 100000, int nbSteps = 200) : base(option)
    {
        this.hestonParameters = hestonParameters;
        this.nbPaths = nbPaths;
        this.nbSteps = nbSteps;

        random = new Random();
    }

    public override double Price()
    {
        return Price(random);
    }

    // Price an Asian option (arithmetic average)
    public double Price(Random? threadRandom)
    {
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

        for (int i = 0; i < this.nbPaths; i++)
        {
            double pathPayoff = 0;

            double[] path = new double[nbSteps];
            double St = S0;
            double Vt = V0;

            for (int step = 0; step < nbSteps; step++)
            {
                double[] sample = RandomNumberGenerator.GenerateCorrelatedNormals(rho, threadRandom);
                double dW1 = sample[0];
                double dW2 = sample[1];

                Vt = Math.Max(0, Vt);
                double sqrtVt = Math.Sqrt(Vt);

                Vt += kappa * (theta - Vt) * dt + sigma * sqrtVt * dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                St *= Math.Exp((r - 0.5 * Vt) * dt + sqrtVt * dW1 * Math.Sqrt(dt));

                path[step] = St;
            }

            // Average price over the period
            double avgPrice = St;//sumPrices / nbSteps;

            // Calculate the payoff
            double payoff = Math.Max(0, avgPrice - K);

            pathPayoff += payoff;

            sumPayoffs += pathPayoff;
        }

        // Discount the average payoff
        double price = Math.Exp(-r * T) * (sumPayoffs / nbPaths);

        return price;
    }

    public double[] Price(int nbPrices)
    {

        double[] prices = new double[nbPrices];
        Parallel.For(0, nbPrices, i =>
        {
            // Generate a new random seed for each thread
            Random threadRandom = new Random(i);
            prices[i] = Price(threadRandom);
        });

        double price = Stats.Mean(prices);
        double stdDev = Stats.StandardDeviation(prices);
        double marginOfError = 1.96 * stdDev / Math.Sqrt(nbPrices);

        return new double[] { price, marginOfError, stdDev };
    }
}