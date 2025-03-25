using HestonPricer.Models;

double r = 0.03;      // Taux sans risque
double S0 = 100.0;    // Prix initial
double K = 100.0;     // Prix d'exercice
double T = 1.0;       // Maturité (en années)
double kappa = 1.5768;   // Taux de retour à la moyenne
double theta = 0.0398;  // Variance à long terme
double vol = 0.316227;     // Volatilité initiale
double sigma = 0.3;   // Volatilité de la volatilité
double rho = -0.5711;    // Corrélation
double lambda = 0.575; // Risk premium of volatiliy 

EuropeanOption o = new(
    S0,
    K,
    T,
    r,
    vol,
    true
);

// BlackScholesPricer bs = new BlackScholesPricer(o);
// double bsPrice = bs.Price();
// Console.WriteLine($"Prix de l'option européenne (BS): {bsPrice}");

HestonParameters hestonParameters = new HestonParameters(
    kappa,
    theta,
    vol * vol,
    sigma,
    rho
);

SemiAnalyticalPricer pricer = new SemiAnalyticalPricer(o, hestonParameters);
string parameter = "Kappa";
double[,] prices = pricer.PriceOverParameter(parameter, 0.2, 10);

for (int i = 0; i < prices.GetLength(0); i++)
{
    Console.WriteLine($"{parameter} : {prices[i, 0]}");
    Console.WriteLine($"Price : {prices[i, 1]}");
}

Derivatives analyzer = new Derivatives(pricer);
double delta = analyzer.FirstOrderDerivative("SpotPrice", 0.0001);
Console.WriteLine($"Delta : {delta}");