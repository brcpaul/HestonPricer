using System.Numerics;

class HestonClosedFormPricer
{
    private double r;
    private double q;
    private double S0;
    private double K;
    private double T;
    private double kappa; // Mean reversion rate
    private double theta; // Long term volatility 
    private double V0;
    private double sigma; // Volatility of variance
    private double lambda; // Risk premium of volatiliy 
    private double rho; // Correlation parameter

    public HestonClosedFormPricer(double r, double q, double S0, double K, double T, double kappa, double theta, double lambda, double V0, double sigma, double rho)
    {
        this.r = r;
        this.q = q;
        this.S0 = S0;
        this.K = K;
        this.T = T;
        this.kappa = kappa;
        this.theta = theta;
        this.lambda = lambda;
        this.V0 = V0;
        this.sigma = sigma;
        this.rho = rho;
    }

    public Complex CharacteristicFunction(Complex phi)
    {

        double b = kappa + lambda;
        Complex i = new Complex(0, 1.0);
        Complex iRhoSigmaPhi = i * rho * sigma * phi;

        Complex d = Complex.Sqrt(Complex.Pow(iRhoSigmaPhi - b, 2) + Math.Pow(sigma, 2) * (phi * phi + i * phi));

        Complex g = (b - iRhoSigmaPhi + d) / (b - iRhoSigmaPhi - d);

        Complex firstTerm = Complex.Pow(Math.Exp(1), r * phi * i * T) * Complex.Pow(S0, i * phi) * Complex.Pow((1 - g * Complex.Pow(Math.Exp(1), d * T)) / (1 - g), -2 * theta * kappa / sigma / sigma);
        Complex expTerm = Complex.Pow(Math.Exp(1), theta * kappa * T / sigma / sigma * (b - iRhoSigmaPhi + d) + V0 / sigma / sigma * (b - iRhoSigmaPhi + d) * ((1 - Complex.Pow(Math.Exp(1), d * T)) / (1 - g * Complex.Pow(Math.Exp(1), d * T))));
        return firstTerm * expTerm;
    }

    public Complex Integrand(double phi)
    {
        Complex i = new Complex(0, 1.0);
        Complex firstTerm = Math.Exp(r * T) * CharacteristicFunction(phi - i);
        Complex secondTerm = -K * CharacteristicFunction(phi);
        Complex denominator = i * phi * Complex.Pow(K, phi * i);
        return (firstTerm + secondTerm) / denominator;
    }

    public double RectangleMethodPrice()
    {
        double maxPhi = 100;
        int N = 100000;
        Complex integral = 0;
        double dPhi = maxPhi / N;

        for (int i = 0; i < N; i++)
        {
            double phi = dPhi * (2 * i + 1) / 2;
            integral += Integrand(phi) * dPhi;
        }


        return (S0 - K * Math.Exp(-r * T))/2.0 + 1.0 / Math.PI * integral.Real;
    }

}