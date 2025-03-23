namespace HestonPricer.Models
{
    public abstract class OptionBase
    {

        public double SpotPrice { get; set; }
        public double Strike { get; set; }
        public double Maturity { get; set; }
        public double RiskFreeRate { get; set; }
        public bool IsCall { get; set; }


        public abstract double Payoff(double[] path);
    }
}
