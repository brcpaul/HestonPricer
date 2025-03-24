namespace HestonPricer.Models
{
    public class EuropeanOption : OptionBase
    {
        public EuropeanOption(double spotPrice, double strike, double maturity, double riskFreeRate, double volatility, bool isCall) : base(spotPrice, strike, maturity, riskFreeRate, volatility, isCall)
        {
        }

        public override double Payoff(double[] path)
        {
            return IsCall
                ? Math.Max(0, path.Last() - Strike)
                : Math.Max(0, Strike - path.Last());
        }
    }
}
