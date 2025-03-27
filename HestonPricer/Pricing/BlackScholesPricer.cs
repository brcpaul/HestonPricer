using HestonPricer.Models;

public class BlackScholesPricer : PricerBase
{

    public BlackScholesPricer(OptionBase option, double volatility) : base(option, volatility) { 

    }
    public override double Price()
    {


        double volatility = Math.Sqrt(hestonParameters.V0);

        double d1 = (Math.Log(option.SpotPrice / option.Strike) + (option.RiskFreeRate + 0.5 * Math.Pow(volatility, 2)) * option.Maturity) /
                    (volatility * Math.Sqrt(option.Maturity));
        double d2 = d1 - volatility * Math.Sqrt(option.Maturity);

        double callPrice = option.SpotPrice * Stats.NormalCDF(d1) - option.Strike * Math.Exp(-option.RiskFreeRate * option.Maturity) * Stats.NormalCDF(d2);
        double putPrice = callPrice - option.SpotPrice + option.Strike * Math.Exp(-option.RiskFreeRate * option.Maturity);

        return option.IsCall ? callPrice : putPrice;
    }

}
