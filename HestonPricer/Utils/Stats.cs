public class Stats
{

    public static double NormalCDF(double x)
    {
        if (x < 0) return 1 - NormalCDF(-x);
        double b1 = 0.31938153;
        double b2 = -0.356563782;
        double b3 = 1.781477937;
        double b4 = -1.821255978;
        double b5 = 1.330274429;
        double k = 1 / (1 + 0.2316419 * x);
        double pdf = Math.Exp(-x * x / 2.0) / Math.Sqrt(2 * Math.PI);
        return 1.0 - pdf * (b1 * k + b2 * Math.Pow(k, 2) + b3 * Math.Pow(k, 3) + b4 * Math.Pow(k, 4) + b5 * Math.Pow(k, 5));
    }

}