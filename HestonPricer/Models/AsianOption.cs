namespace HestonPricer.Models
{
    public class AsianOption : OptionBase
    {

        public override double Payoff(double[] path)
        {
            return IsCall
                ? Math.Max(0, path.Average() - Strike)
                : Math.Max(0, Strike - path.Average());
        }
    }
}
