using HestonPricer.Models;

public class Derivatives
{
    private PricerBase pricer;

    public Derivatives(PricerBase pricer)
    {
        this.pricer = pricer;
    }

    public double FirstOrderDerivative(string variable, double h = 0.0001)
    {
        double originalValue = GetVariableValue(variable);
        SetVariableValue(variable, originalValue + h);
        double valuePlusH = pricer.Price();
        SetVariableValue(variable, originalValue - h);
        double valueMinusH = pricer.Price();
        SetVariableValue(variable, originalValue);
        return (valuePlusH - valueMinusH) / (2 * h);
    }

    public double SecondOrderDerivative(string variable, double h = 0.0001)
    {
        double originalValue = GetVariableValue(variable);
        SetVariableValue(variable, originalValue + h);
        double valuePlusH = pricer.Price();
        SetVariableValue(variable, originalValue - h);
        double valueMinusH = pricer.Price();
        SetVariableValue(variable, originalValue);
        double valueOriginal = pricer.Price();
        return (valuePlusH - 2 * valueOriginal - valueMinusH) / (h * h);
    }

    private double GetVariableValue(string variable)
    {
        switch (variable)
        {
            case "SpotPrice":
                return pricer.Option.SpotPrice;
            case "Strike":
                return pricer.Option.Strike;
            case "Maturity":
                return pricer.Option.Maturity;
            case "RiskFreeRate":
                return pricer.Option.RiskFreeRate;
            default:
                throw new ArgumentException("Invalid variable name");
        }
    }

    private void SetVariableValue(string variable, double value)
    {
        switch (variable)
        {
            case "SpotPrice":
                pricer.Option.SpotPrice = value;
                break;
            case "Strike":
                pricer.Option.Strike = value;
                break;
            case "Maturity":
                pricer.Option.Maturity = value;
                break;
            case "RiskFreeRate":
                pricer.Option.RiskFreeRate = value;
                break;
            default:
                throw new ArgumentException("Invalid variable name");
        }
    }
}