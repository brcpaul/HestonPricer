namespace HestonPricer.Models
{
    public abstract class OptionBase
    {

        public double SpotPrice { get; set; }
        public double Strike { get; set; }
        public double Maturity { get; set; }
        public double RiskFreeRate { get; set; }
        public double? Volatility { get; set; }
        public bool IsCall { get; set; }

        public OptionBase(
            double spotPrice,
            double strike,
            double maturity,
            double riskFreeRate,
            double? volatility,
            bool isCall
        )
        {
            SpotPrice = spotPrice;
            Strike = strike;
            Maturity = maturity;
            RiskFreeRate = riskFreeRate;
            Volatility = volatility;
            IsCall = isCall;
        }

        public abstract double Payoff(double[] path);
    }
}
