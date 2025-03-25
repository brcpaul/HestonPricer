namespace HestonTests;

public class TestsRandom {

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestRandomCorrelated(){

        Assert.Throws<ArgumentOutOfRangeException>(() => RandomNumberGenerator.GenerateCorrelatedNormals(1.5));
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomNumberGenerator.GenerateCorrelatedNormals(-1.5));

        int n = 100000;
        double[] a = new double[n];
        double[] b = new double[n];

        for(int i=0;i<n;i++) {
            double[] sample = RandomNumberGenerator.GenerateCorrelatedNormals(-0.25);
            a[i] = sample[0];
            b[i] = sample[1];
        }

        double correlation = Stats.Correlation(a, b);
        Assert.That(correlation, Is.EqualTo(-0.25).Within(0.05));
    }


}