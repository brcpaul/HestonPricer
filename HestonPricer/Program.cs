// See https://aka.ms/new-console-template for more information
using HestonPricer;

// Initialisation des paramètres
double r = 0.05;      // Taux sans risque
double q = 0.00;      // Rendement du dividende
double S0 = 100.0;    // Prix initial
double K = 100.0;     // Prix d'exercice
double T = 1.0;       // Maturité (en années)
double kappa = 2.0;   // Taux de retour à la moyenne
double theta = 0.09;  // Variance à long terme
double V0 = 0.09;     // Variance initiale
double sigma = 0.1;   // Volatilité de la volatilité
double rho = -0.7;    // Corrélation

// Création du pricer
var pricer = new HestonAsianPricer(r, q, S0, K, T, kappa, theta, V0, sigma, rho);

// Prix d'une option d'achat asiatique
double callPrice = pricer.PriceAsianOption(OptionType.Call);
Console.WriteLine($"Prix d'option d'achat asiatique: {callPrice}");

// Prix d'une option de vente asiatique
double putPrice = pricer.PriceAsianOption(OptionType.Put);
Console.WriteLine($"Prix d'option de vente asiatique: {putPrice}");

// Avec intervalle de confiance (95%)
var (price, error, lower, upper) = pricer.PriceWithConfidenceInterval(OptionType.Call);
Console.WriteLine($"Prix avec IC 95%: {price} ± {error} [{lower} - {upper}]");

