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
    true
);

// BlackScholesPricer bs = new BlackScholesPricer(o);
// double bsPrice = bs.Price();
// Console.WriteLine($"Prix de l'option européenne (BS): {bsPrice}");

HestonParameters hestonParameters = new HestonParameters(
    kappa*0,
    theta,
    vol * vol,
    sigma*0,
    rho
);

// BlackScholesPricer pricer = new BlackScholesPricer(o);
// string parameter = "Delta";
// double[,] prices = pricer.PriceOverParameter(parameter, 0.2, 10);

// for (int i = 0; i < prices.GetLength(0); i++)
// {
//     Console.WriteLine($"{parameter} : {prices[i, 0]}");
//     Console.WriteLine($"Price : {prices[i, 1]}");
// }

MonteCarloPricer pricer = new MonteCarloPricer(o, hestonParameters, 10000, 100);
BlackScholesPricer pricerBS = new BlackScholesPricer(o, vol);

// Console.WriteLine(pricer.Price(10));

// double[] result = pricer.FirstOrderDerivative("SpotPrice", 1, 100);
// Console.WriteLine($"Delta: {result[0]}");
// Console.WriteLine($"Intervalle de confiance : {result[1]}");


// Console.WriteLine($"Prix de l'option européenne (Heston): {result[0]}");
// Console.WriteLine($"Intervalle de confiance : {result[1]}");
// Console.WriteLine($"Ecart type: {result[2]}");


var stopwatch = System.Diagnostics.Stopwatch.StartNew();

double[,] sensi = pricer.SensiOverParameter("SpotPrice", "SpotPrice", 50, 5, 10);

stopwatch.Stop();
Console.WriteLine($"Time taken for Heston SensiOverParameter: {stopwatch.ElapsedMilliseconds} ms");

stopwatch.Restart();

double[,] sensiBS = pricerBS.SensiOverParameter("SpotPrice", "SpotPrice", 80, 120, 5);

stopwatch.Stop();
Console.WriteLine($"Time taken for Black-Scholes SensiOverParameter: {stopwatch.ElapsedMilliseconds} ms");

Console.WriteLine(sensi[0, 0]);

// for (int i = 0; i < sensi.GetLength(0); i++)
// {
//     Console.WriteLine($"SpotPrice : {sensi[i, 0]}");
//     Console.WriteLine($"Delta : {sensi[i, 1]}");
// }