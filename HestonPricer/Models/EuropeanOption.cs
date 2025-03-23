namespace HestonPricer.Models
{
    public class EuropeanOption : OptionBase
    {
        public override double Payoff(double[] path)
        {
            return IsCall
                ? Math.Max(0, path.Last() - Strike)
                : Math.Max(0, Strike - path.Last());
        }
    }
}
