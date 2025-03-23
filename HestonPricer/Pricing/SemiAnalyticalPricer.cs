using System.Numerics;
using HestonPricer.Models;

class SemiAnalyticalPricer
{

    private OptionBase option;
    private HestonParameters hestonParameters;

    public SemiAnalyticalPricer(OptionBase option, HestonParameters hestonParameters)
    {
        this.option = option;
        this.hestonParameters = hestonParameters;
    }

    private Complex CharacteristicFunction(Complex phi)
    {

        double b = hestonParameters.Kappa + hestonParameters.Lambda;
        double kappa = hestonParameters.Kappa;
        double theta = hestonParameters.Theta;
        double sigma = hestonParameters.Sigma;
        double rho = hestonParameters.Rho;
        double V0 = hestonParameters.V0;
        double r = option.RiskFreeRate;
        double T = option.Maturity;
        double S0 = option.Strike;
        double K = option.Strike;


        Complex i = new Complex(0, 1.0);
        Complex iRhoSigmaPhi = i * rho * sigma * phi;

        Complex d = Complex.Sqrt(Complex.Pow(iRhoSigmaPhi - b, 2) + Math.Pow(sigma, 2) * (phi * phi + i * phi));

        Complex g = (b - iRhoSigmaPhi + d) / (b - iRhoSigmaPhi - d);

        Complex firstTerm = Complex.Pow(Math.Exp(1), r * phi * i * T) * Complex.Pow(S0, i * phi) * Complex.Pow((1 - g * Complex.Pow(Math.Exp(1), d * T)) / (1 - g), -2 * theta * kappa / sigma / sigma);
        Complex expTerm = Complex.Pow(Math.Exp(1), theta * kappa * T / sigma / sigma * (b - iRhoSigmaPhi + d) + V0 / sigma / sigma * (b - iRhoSigmaPhi + d) * ((1 - Complex.Pow(Math.Exp(1), d * T)) / (1 - g * Complex.Pow(Math.Exp(1), d * T))));
        return firstTerm * expTerm;
    }

    private Complex Integrand(double phi)
    {

        double r = option.RiskFreeRate;
        double T = option.Maturity;
        double K = option.Strike;

        Complex i = new Complex(0, 1.0);
        Complex firstTerm = Math.Exp(r * T) * CharacteristicFunction(phi - i);
        Complex secondTerm = -K * CharacteristicFunction(phi);
        Complex denominator = i * phi * Complex.Pow(K, phi * i);
        return (firstTerm + secondTerm) / denominator;
    }

    public double Price()
    {

        double r = option.RiskFreeRate;
        double T = option.Maturity;
        double K = option.Strike;
        double S0 = option.Strike;

        double maxPhi = 1000;
        int N = 100000;
        Complex integral = 0;
        double dPhi = maxPhi / N;

        for (int i = 0; i < N; i++)
        {
            double phi = dPhi * (2 * i + 1) / 2;
            integral += Integrand(phi) * dPhi;
        }

        return (S0 - K * Math.Exp(-r * T)) / 2.0 + 1.0 / Math.PI * integral.Real;
    }

}