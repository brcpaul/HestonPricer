using HestonPricer.Models;

public abstract class PricerBase
{

    protected OptionBase option;
    protected HestonParameters? hestonParameters;
    public PricerBase(OptionBase option)
    {
        this.option = option;
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

    public double[,] PriceOverParameter(string parameter, double range, int steps)
    {
        double[,] result = new double[steps, 2];
        double originalValue = GetVariableValue(parameter);
        for (int i = 0; i < steps; i += 1)
        {
            SetVariableValue(parameter, originalValue + range * ((double)i / steps - 0.5));
            result[i, 0] = GetVariableValue(parameter);
            result[i, 1] = Price();
        }
        SetVariableValue(parameter, originalValue);
        return result;
    }

    private double GetVariableValue(string variable)
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

    private void SetVariableValue(string variable, double value)
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
}