namespace HestonPricer.Models
{
    public class HestonParameters
    {

        public double Kappa { get; set; }
        public double Theta { get; set; }
        public double Sigma { get; set; }
        public double Rho { get; set; }
        public double V0 { get; set; }
        public double Lambda { get; set; } = 0.0;
    }
}
