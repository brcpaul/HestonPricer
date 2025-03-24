namespace HestonPricer.Models
{
    public class HestonParameters
    {
        public HestonParameters(double kappa, double theta, double v0, double sigma, double rho, double lambda = 0)
        {
            Kappa = kappa;
            Theta = theta;
            V0 = v0;
            Sigma = sigma;
            Rho = rho;
            Lambda = lambda;
        }

        public double Kappa { get; set; }
        public double Theta { get; set; }
        public double Sigma { get; set; }
        public double Rho { get; set; }
        public double V0 { get; set; }
        public double Lambda { get; set; }
    }
}
