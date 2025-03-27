using ExcelDna.Integration;
using HestonPricer.Models;

public class ExcelFunctions
{
    [ExcelFunction(Name = "PriceHestonSingle")]
    public static double PriceHeston(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 100000, int nbSteps = 200)
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

        HestonParameters hestonParameters = new HestonParameters(kappa, theta, v0 * v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(option, hestonParameters, nbPaths, nbSteps);
        return pricer.Price();
    }

    [ExcelFunction(Name = "PriceHestonMultiple")]
    public static double[] PriceHeston(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 1000, int nbSteps = 200, int nbPrices = 100)
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

        HestonParameters hestonParameters = new HestonParameters(kappa, theta, v0 * v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(option, hestonParameters, nbPaths, nbSteps);
        return pricer.Price(nbPrices);
    }

    [ExcelFunction(Name = "PriceHestonParams")]
    public static double[,] PriceOverParameterHeston(string parameter, double range, int steps, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 100000, int nbSteps = 100)
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

        HestonParameters hestonParameters = new HestonParameters(kappa, theta, v0 * v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(option, hestonParameters, nbPaths, nbSteps);
        return pricer.PriceOverParameter(parameter, range, steps);
    }

    [ExcelFunction(Name = "PriceBSParams")]
    public static double[,] PriceOverParameterBS(string parameter, double range, int steps, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double volatility)
    {
        OptionBase option;
        switch (optionType)
        {
            case "EuropeanCall":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, volatility, true);
                break;
            case "EuropeanPut":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, volatility, false);
                break;
            case "AsianCall":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, volatility, true);
                break;
            case "AsianPut":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, volatility, false);
                break;
            default:
                throw new ArgumentException("Invalid option type");
        }

        BlackScholesPricer pricer = new BlackScholesPricer(option);
        return pricer.PriceOverParameter(parameter, range, steps);
    }
}
