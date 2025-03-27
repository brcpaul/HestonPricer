namespace HestonPricer.Models
{
    public class AsianOption : OptionBase
    {
        public AsianOption(double spotPrice, double strike, double maturity, double riskFreeRate, double? volatility, bool isCall) : base(spotPrice, strike, maturity, riskFreeRate, volatility, isCall)
        {
        }

        public override double Payoff(double[] path)
        {
            return IsCall
                ? Math.Max(0, Stats.Mean(path) - Strike)
                : Math.Max(0, Strike - Stats.Mean(path));
        }
    }
}
