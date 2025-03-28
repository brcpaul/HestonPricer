using HestonPricer.Models;

public class MonteCarloPricer : PricerBase
{
    private int nbPaths;       // Number of Monte Carlo paths
    private int nbSteps;       // Number of time steps for discretization
    private Random random;

    public MonteCarloPricer(OptionBase option, HestonParameters hestonParameters, int nbPaths = 100000, int nbSteps = 200) : base(option, hestonParameters)
    {
        this.nbPaths = nbPaths;
        this.nbSteps = nbSteps;

        random = new Random();
    }

    public override double Price()
    {
        return Price(random);
    }

    public double Price(Random? threadRandom)
    {
        double T = option.Maturity;
        double S0 = option.SpotPrice;
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

            double[][] path = new double[2][];
            path[0] = new double[nbSteps];
            path[1] = new double[nbSteps];

            double[] St = {S0, S0};
            double[] Vt = {V0, V0};

            for (int step = 0; step < nbSteps; step++)
            {
                double[] sample = RandomNumberGenerator.GenerateCorrelatedNormals(rho, threadRandom);
                double dW1 = sample[0];
                double dW2 = sample[1];
                double antiDW1 = -dW1;
                double antiDW2 = -dW2;

                Vt[0] = Math.Max(0, Vt[0]);
                Vt[1] = Math.Max(0, Vt[1]);
                double sqrtVt0 = Math.Sqrt(Vt[0]);
                double sqrtVt1 = Math.Sqrt(Vt[1]);  

                Vt[0] += kappa * (theta - Vt[0]) * dt + sigma * sqrtVt0 * dW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (dW2 * dW2 - 1);
                St[0] *= Math.Exp((r - 0.5 * Vt[0]) * dt + sqrtVt0 * dW1 * Math.Sqrt(dt));

                Vt[1] += kappa * (theta - Vt[1]) * dt + sigma * sqrtVt1 * antiDW2 * Math.Sqrt(dt) + 0.25 * sigma * sigma * dt * (antiDW2 * antiDW2 - 1);
                St[1] *= Math.Exp((r - 0.5 * Vt[1]) * dt + sqrtVt1 * antiDW1 * Math.Sqrt(dt));

                path[0][step] = St[0];
                path[1][step] = St[1];
            }

            sumPayoffs += (option.Payoff(path[0]) + option.Payoff(path[1]))/2;
        }

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

    public double FirstOrderDerivative(string variable, double h = 0.0001, Random? threadRandom = null)
    {
        double originalValue = GetVariableValue(variable);
        SetVariableValue(variable, originalValue + h);
        double valuePlusH = Price(threadRandom);
        SetVariableValue(variable, originalValue - h);
        double valueMinusH = Price(threadRandom);
        SetVariableValue(variable, originalValue);
        return (valuePlusH - valueMinusH) / (2 * h);
    }


    public double[,] PriceOverParameter(string parameter, double min, double max, int steps, int nbPrices)
    {
        double[,] result = new double[steps+1, 3];
        double originalValue = GetVariableValue(parameter);
        for (int i = 0; i < steps+1; i += 1)
        {
            SetVariableValue(parameter, min + (max-min)* ((double)i / steps));
            result[i, 0] = GetVariableValue(parameter);
            double[] priceInfos = Price(nbPrices);
            result[i, 1] = priceInfos[0];
            result[i, 2] = priceInfos[1];
        }
        SetVariableValue(parameter, originalValue);
        return result;
    }

    public double[,] SensiOverParameter(string sensi, string parameter, double min, double max, int steps, int nbDraws) {
        double[,] result = new double[steps+1, 3];
        double originalValue = GetVariableValue(parameter);
        for (int i = 0; i < steps+1; i += 1)
        {
            SetVariableValue(parameter, min + (max-min) * ((double)i / steps));
            result[i, 0] = GetVariableValue(parameter);
            double[] fod = FirstOrderDerivative(sensi, 1, nbDraws);
            result[i, 1] = fod[0];
            result[i, 2] = fod[1];
        }
        SetVariableValue(parameter, originalValue);
        return result;
    }
}