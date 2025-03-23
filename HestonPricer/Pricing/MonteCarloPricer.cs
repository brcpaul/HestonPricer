using HestonPricer.Models;

public class MonteCarloPricer
{
    private OptionBase option;
    private HestonParameters hestonParameters;
    private int nbPaths;       // Number of Monte Carlo paths
    private int nbSteps;       // Number of time steps for discretization
                               // private bool antithetic;   // Use antithetic variates for variance reduction

    private Random random;

    public MonteCarloPricer(OptionBase option, HestonParameters hestonParameters, int nbPaths = 100000, int nbSteps = 200)
    {
        this.option = option;
        this.hestonParameters = hestonParameters;
        this.nbPaths = nbPaths;
        this.nbSteps = nbSteps;

        random = new Random();
    }

    // Price an Asian option (arithmetic average)
    public double PriceHeston()
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

        for (int i = 0; i < nbPaths; i++)
        {
            double pathPayoff = 0;

            double St = S0;
            double Vt = V0;
            double[] path = new double[nbSteps];

            for (int step = 0; step < nbSteps; step++)
            {
                // Generate correlated random variables
                double[] normals = RandomNumberGenerator.GenerateCorrelatedNormals(rho);
                double dW1 = normals[0];
                double dW2 = normals[1];

                // Ensure variance remains positive
                Vt = Math.Max(0, Vt);
                double sqrtVt = Math.Sqrt(Vt);

                Vt += kappa * (theta - Vt) * dt + sigma * sqrtVt * dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                St *= Math.Exp((r - 0.5 * Vt) * dt + sqrtVt * dW1 * Math.Sqrt(dt));

                path[step] = St;
            }

            pathPayoff += option.Payoff(path);

            sumPayoffs += pathPayoff;
        }

        // Discount the average payoff
        double price = Math.Exp(-r * T) * (sumPayoffs / nbPaths);
        return price;
    }
}