using HestonPricer.Models;

public class Derivatives
{
    private PricerBase pricer;

    public Derivatives(PricerBase pricer)
    {
        this.pricer = pricer;
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