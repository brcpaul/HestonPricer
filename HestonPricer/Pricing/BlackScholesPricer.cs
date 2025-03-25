using HestonPricer.Models;

public class BlackScholesPricer : PricerBase
{

    public BlackScholesPricer(OptionBase option) : base(option) { }
    public override double Price()
    {
        double d1 = (Math.Log(option.SpotPrice / option.Strike) + (option.RiskFreeRate + 0.5 * Math.Pow(option.Volatility, 2)) * option.Maturity) /
                    (option.Volatility * Math.Sqrt(option.Maturity));
        double d2 = d1 - option.Volatility * Math.Sqrt(option.Maturity);

        double callPrice = option.SpotPrice * Stats.NormalCDF(d1) - option.Strike * Math.Exp(-option.RiskFreeRate * option.Maturity) * Stats.NormalCDF(d2);
        double putPrice = callPrice - option.SpotPrice + option.Strike * Math.Exp(-option.RiskFreeRate * option.Maturity);

        return option.IsCall ? callPrice : putPrice;
    }

}
