using ExcelDna.Integration;
using HestonPricer.Models;

struct PricerParameters
{
    public OptionBase option;
    public HestonParameters hestonParameters;
}

public class ExcelFunctions
{

    private static PricerParameters ParseParams(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho)
    {
        OptionBase option;
        switch (optionType)
        {
            case "EuropeanCall":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, true);
                break;
            case "EuropeanPut":
                option = new EuropeanOption(spotPrice, strike, maturity, riskFreeRate, false);
                break;
            case "AsianCall":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, true);
                break;
            case "AsianPut":
                option = new AsianOption(spotPrice, strike, maturity, riskFreeRate, false);
                break;
            default:
                throw new ArgumentException("Invalid option type");
        }

        HestonParameters hestonParameters = new HestonParameters(kappa, theta, v0 * v0, sigma, rho);
        return new PricerParameters { option = option, hestonParameters = hestonParameters };
    }

    [ExcelFunction(Name = "PriceHestonSingle")]
    public static double PriceHeston(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 100000, int nbSteps = 200)
    {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, kappa, theta, v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(parameters.option, parameters.hestonParameters, nbPaths, nbSteps);
        return pricer.Price();
    }

    [ExcelFunction(Name = "PriceHestonMultiple")]
    public static double[] PriceHeston(string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 1000, int nbSteps = 200, int nbPrices = 100)
    {
        
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, kappa, theta, v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(parameters.option, parameters.hestonParameters, nbPaths, nbSteps);
        return pricer.Price(nbPrices);
    }

    [ExcelFunction(Name = "FirstOrderDerivativeHeston")] 
    public static double[] FirstOrderDerivativeHeston(string parameter, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 100000, int nbSteps = 200)
    {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, kappa, theta, v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(parameters.option, parameters.hestonParameters, nbPaths, nbSteps);
        return pricer.FirstOrderDerivative(parameter, 0.1, 10);
    }

    [ExcelFunction(Name = "PriceHestonParams")]
    public static double[,] PriceOverParameterHeston(string parameter, double min, double max, int steps, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 10, int nbSteps=100, int nbPrices=10)
    {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, kappa, theta, v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(parameters.option, parameters.hestonParameters, nbPaths, nbSteps);
        return pricer.PriceOverParameter(parameter, min, max, steps, nbPrices);
    }

    [ExcelFunction(Name = "PriceBSParams")]
    public static double[,] PriceOverParameterBS(string parameter, double min, double max, int steps, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double volatility)
    {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, 0, 0, 0, 0, 0);
        BlackScholesPricer pricer = new BlackScholesPricer(parameters.option, volatility);
        return pricer.PriceOverParameter(parameter, min, max, steps);
    }

    [ExcelFunction(Name = "SensiHestonParams")] 
    public static double[,] SensiOverParameterHeston(string sensi, string parameter, double min, double max, int steps, string optionType ,double spotPrice, double strike, double maturity, double riskFreeRate, double kappa, double theta, double v0, double sigma, double rho, int nbPaths = 10, int nbSteps=100, int nbPrices=1) {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, kappa, theta, v0, sigma, rho);
        MonteCarloPricer pricer = new MonteCarloPricer(parameters.option, parameters.hestonParameters, nbPaths, nbSteps);
        return pricer.SensiOverParameter(sensi, parameter, min, max, steps, nbPrices);
    }

    [ExcelFunction(Name = "SensiBSParams")]
    public static double[,] SensiOverParameterBS(string sensi, string parameter, double min, double max, int steps, string optionType, double spotPrice, double strike, double maturity, double riskFreeRate, double volatility)
    {
        PricerParameters parameters = ParseParams(optionType, spotPrice, strike, maturity, riskFreeRate, 0, 0, 0, 0, 0);
        BlackScholesPricer pricer = new BlackScholesPricer(parameters.option, volatility);
        return pricer.SensiOverParameter(sensi, parameter, min, max, steps);
    }
}
