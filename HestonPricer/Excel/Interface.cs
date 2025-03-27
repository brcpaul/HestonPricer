using ExcelDna.Integration;
using HestonPricer.Models;

public class ExcelFunctions
{
    [ExcelFunction(Description = "Price an option using Heston model")]
    public static double PriceOptionHeston(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 100000, int nbSteps = 200)
    {
        OptionBase option;
        switch (optionType)
        {
            case "EuropeanCall":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, null, true);
                break;
            case "EuropeanPut":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, null, false);
                break;
            case "AsianCall":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, null, true);
                break;
            case "AsianPut":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, null, false);
                break;
            default:
                throw new ArgumentException("Invalid option type");
        }


        HestonParameters hestonParameters = new HestonParameters(kappa, theta, v0*v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(option, hestonParameters, nbPaths, nbSteps);
        return pricer.Price();
    }
}