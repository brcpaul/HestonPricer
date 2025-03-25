using HestonPricer;
namespace HestonTests;

public class TestsStats
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestNormalCDF()
    {
        double x = 1.0;
        double expected = 0.8413;
        double result = Stats.NormalCDF(1.0);
        Assert.That(result, Is.EqualTo(expected).Within(0.0001));
    }

    [Test]
    public void TestStandardDeviation()
    {
        double[] data = { 1.0, 2.0, 3.0, 4.0, 5.0 };
        double expected = 1.5811;
        double result = Stats.StandardDeviation(data);
        Assert.That(result, Is.EqualTo(expected).Within(0.001));
    }

    [Test]
    public void TestCorrelation()
    {
        double[] a = { 1.0, 2.0, 3.0, 4.0, 5.0 };
        double[] b = { 2.0, 4.0, 6.0, 8.0, 10.0 };
        double expected = 1.0;
        double result = Stats.Correlation(a, b);
        Assert.That(result, Is.EqualTo(expected).Within(0.0001));
    }
}
