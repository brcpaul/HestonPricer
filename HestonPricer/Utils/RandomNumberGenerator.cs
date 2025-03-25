public static class RandomNumberGenerator
{
    private static Random random = new Random();

    public static double[] GenerateCorrelatedNormals(double rho, Random? customRandom = null)
    {
        Random random = customRandom ?? RandomNumberGenerator.random;

        if (rho < -1 || rho > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rho), "Correlation coefficient must be between -1 and 1.");
        }
        // Box-Muller transform to generate two independent standard normal variables
        double u1 = random.NextDouble();
        double u2 = random.NextDouble();
        double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        double z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        double x = z0;
        double y = rho * z0 + Math.Sqrt(1 - rho * rho) * z1;

        return new double[] { x, y };
    }

    public static void SetRandom(Random random)
    {
        RandomNumberGenerator.random = random;
    }
}