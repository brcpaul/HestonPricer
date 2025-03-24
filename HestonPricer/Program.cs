using HestonPricer.Models;

double r = 0.03;      // Taux sans risque
double q = 0.00;      // Rendement du dividende
double S0 = 100.0;    // Prix initial
double K = 100.0;     // Prix d'exercice
double T = 1.0;       // Maturité (en années)
double kappa = 1.5768;   // Taux de retour à la moyenne
double theta = 0.0398;  // Variance à long terme
double vol = 0.31;     // Volatilité initiale
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

HestonParameters hestonParameters = new(
    kappa * 0,
    theta * 0,
    vol * vol,
    sigma * 0,
    rho * 0
);

// HestonClosedFormPricer pricer = new HestonClosedFormPricer(r, q, S0, K, T, kappa, theta, lambda, V0, sigma, rho);
// double price = pricer.RectangleMethodPrice();
// Console.WriteLine($"Prix de l'option asiatique: {price}");


var stopwatch = System.Diagnostics.Stopwatch.StartNew();

double[] result = new MonteCarloPricer(
    o, hestonParameters, 1000, 100
).Price(100);

stopwatch.Stop();
Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");

Console.WriteLine($"Prix de l'option asiatique: {result[0]}");
Console.WriteLine($"Marge d'erreur à 95%: {result[1]}");
Console.WriteLine("Std Dev: " + result[2]);