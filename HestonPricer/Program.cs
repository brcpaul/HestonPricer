
using ExcelDna.Integration;

double r = 0.03;      // Taux sans risque

Console.WriteLine("test");
public static class MyFunctions
{
    [ExcelFunction(Description = "My first .NET function")]
    public static string SayHello(string name)
    {
        return "Hello " + name;
    }
}


// // Initialisation des paramètres
// double q = 0.00;      // Rendement du dividende
// double S0 = 100.0;    // Prix initial
// double K = 100.0;     // Prix d'exercice
// double T = 1.0;       // Maturité (en années)
// double kappa = 1.5768;   // Taux de retour à la moyenne
// double theta = 0.0398;  // Variance à long terme
// double V0 = 0.1;     // Variance initiale
// double sigma = 0.3;   // Volatilité de la volatilité
// double rho = -0.5711;    // Corrélation
// double lambda = 0.575; // Risk premium of volatiliy 


// HestonClosedFormPricer pricer = new HestonClosedFormPricer(r, q, S0, K, T, kappa, theta, lambda, V0, sigma, rho);
// double price = pricer.RectangleMethodPrice();
// Console.WriteLine($"Prix de l'option asiatique: {price}");

// // Création du pricer
// var pricerMC = new HestonAsianPricer(r, q, S0, K, T, kappa, theta, V0, sigma, rho, 1000000);

// // Prix d'une option d'achat asiatique
// double callPrice = pricerMC.PriceAsianOption(OptionType.Call);
// Console.WriteLine($"Prix d'option d'achat asiatique: {callPrice}");


