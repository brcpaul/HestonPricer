using HestonPricer.Models;

public abstract class PricerBase
{

    protected OptionBase option;
    protected HestonParameters hestonParameters;
    public PricerBase(OptionBase option, double volatility)
    {
        this.option = option;
        hestonParameters = new HestonParameters(0, 0, volatility*volatility, 0, 0);
    }
    public PricerBase(OptionBase option, HestonParameters hestonParameters)
    {
        this.option = option;
        this.hestonParameters = hestonParameters;
    }

    public OptionBase Option
    {
        get { return option; }
    }

    public abstract double Price();

    public double[,] PriceOverParameter(string parameter, double min, double max, int steps)
    {
        double[,] result = new double[steps+1, 2];
        double originalValue = GetVariableValue(parameter);
        for (int i = 0; i < steps+1; i += 1)
        {
            SetVariableValue(parameter, min + (max-min) * ((double)i / steps - 0.5));
            result[i, 0] = GetVariableValue(parameter);
            result[i, 1] = Price();
        }
        SetVariableValue(parameter, originalValue);
        return result;
    }

    public double[,] SensiOverParameter(string sensi, string parameter, double min, double max, int steps) {
        double[,] result = new double[steps+1, 2];
        double originalValue = GetVariableValue(parameter);
        for (int i = 0; i < steps+1; i += 1)
        {
            SetVariableValue(parameter, min + (max-min) * ((double)i / steps - 0.5));
            result[i, 0] = GetVariableValue(parameter);
            result[i, 1] = FirstOrderDerivative(sensi,0.01);
        }
        SetVariableValue(parameter, originalValue);
        return result;
    }

    protected double GetVariableValue(string variable)
    {
        switch (variable)
        {
            case "SpotPrice":
                return option.SpotPrice;
            case "Strike":
                return option.Strike;
            case "Maturity":
                return option.Maturity;
            case "RiskFreeRate":
                return option.RiskFreeRate;
            case "Kappa":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                return hestonParameters.Kappa;
            case "Theta":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                return hestonParameters.Theta;
            case "Sigma":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                return hestonParameters.Sigma;
            case "V0":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                return hestonParameters.V0;
            case "Rho":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                return hestonParameters.Rho;
            default:
                throw new ArgumentException("Invalid variable name");
        }
    }

    protected void SetVariableValue(string variable, double value)
    {
        switch (variable)
        {
            case "SpotPrice":
                option.SpotPrice = value;
                break;
            case "Strike":
                option.Strike = value;
                break;
            case "Maturity":
                option.Maturity = value;
                break;
            case "RiskFreeRate":
                option.RiskFreeRate = value;
                break;
            case "Kappa":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                hestonParameters.Kappa = value;
                break;
            case "Theta":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                hestonParameters.Theta = value;
                break;
            case "Sigma":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                hestonParameters.Sigma = value;
                break;
            case "V0":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                hestonParameters.V0 = value;
                break;
            case "Rho":
                if (hestonParameters == null)
                    throw new ArgumentException("Heston parameters not set");
                hestonParameters.Rho = value;
                break;
            default:
                throw new ArgumentException("Invalid variable name");
        }
    }

    public double FirstOrderDerivative(string variable, double h = 0.0001)
    {
        double originalValue = GetVariableValue(variable);
        SetVariableValue(variable, originalValue + h);
        double valuePlusH = Price();
        SetVariableValue(variable, originalValue - h);
        double valueMinusH = Price();
        SetVariableValue(variable, originalValue);
        return (valuePlusH - valueMinusH) / (2 * h);
    }

    public virtual double[] FirstOrderDerivative(string variabe, double h=0.0001, int nbDraws=10){
        double[] results = new double[nbDraws];
        for(int i = 0; i < nbDraws; i++){
            results[i] += FirstOrderDerivative(variabe, h);
        }
        double std = Stats.StandardDeviation(results);
        double marginOfError = 1.96 * std / Math.Sqrt(nbDraws);
        return new double[] {results.Average(), marginOfError, std};
    }

    public double SecondOrderDerivative(string variable , double h = 0.0001)
    {
        double originalValue = GetVariableValue(variable);
        SetVariableValue(variable, originalValue + h);
        double valuePlusH = Price();
        SetVariableValue(variable, originalValue - h);
        double valueMinusH = Price();
        SetVariableValue(variable, originalValue);
        double valueOriginal = Price();
        return (valuePlusH - 2 * valueOriginal - valueMinusH) / (h * h);
    }
}