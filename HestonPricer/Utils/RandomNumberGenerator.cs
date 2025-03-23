public static class RandomNumberGenerator
{
    private static readonly Random random = new Random();

    public static double[] GenerateCorrelatedNormals(double rho)
    {
        double u1 = random.NextDouble();
        double u2 = random.NextDouble();
        double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        double z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        double x = z0;
        double y = rho * z0 + Math.Sqrt(1 - rho * rho) * z1;

        return new double[] { x, y };
    }
}