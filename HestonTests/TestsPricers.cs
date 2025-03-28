using HestonPricer.Models;
namespace HestonTests;

public class TestsPricers
{
    private OptionBase option;
    private HestonParameters hestonParams;
    private HestonParameters hestonAsBlackScholesParams;
    private BlackScholesPricer bsPricer;
    private MonteCarloPricer mcPricer;
    private MonteCarloPricer mcBsPricer;
    private SemiAnalyticalPricer saPricer;
    private SemiAnalyticalPricer saBsPricer;

    [SetUp]
    public void Setup()
    {
        option = new EuropeanOption(100, 100, 1, 0.03, true);
        hestonParams = new HestonParameters(1.5768, 0.0398, 0.1, 0.3, -0.5711, 0.575);
        hestonAsBlackScholesParams = new HestonParameters(0, 0, 0.1, 0, 0, 0);

        bsPricer = new BlackScholesPricer(option, 0.316);

        saPricer = new SemiAnalyticalPricer(option, hestonParams);
        saBsPricer = new SemiAnalyticalPricer(option, hestonAsBlackScholesParams);

        mcPricer = new MonteCarloPricer(option, hestonParams, 10000, 500);
        mcBsPricer = new MonteCarloPricer(option, hestonAsBlackScholesParams, 10000, 500);
    }

    [Test]
    public void TestBlackScholesPrice()
    {
        double expected = 13.90;
        double result = bsPricer.Price();
        Assert.That(result, Is.EqualTo(expected).Within(0.01));
    }

    [Test]
    public void TestMonteCarloBSPrice()
    {
        double expected = 13.90;
        double[] result = mcBsPricer.Price(100);
        Assert.That(result[0], Is.EqualTo(expected).Within(0.02));
    }

    [Test]
    public void TestMonteCarloHestonPrice()
    {
        double expected = 11.54;
        double[] result = mcPricer.Price(100);
        Assert.That(result[0], Is.EqualTo(expected).Within(0.02));
    }

    [Test]
    public void TestSemiAnalyticalBSPrice()
    {
        double expected = 13.90;
        double result = saBsPricer.Price();
        Assert.That(result, Is.EqualTo(expected).Within(0.01));
    }

    [Test]
    public void TestSemiAnalyticalHestonPrice()
    {
        double expected = 11.54;
        double result = saPricer.Price();
        Assert.That(result, Is.EqualTo(expected).Within(0.01));
    }

    [Test]
    public void TestBlackScholesFirstDerivatives(){
        double expectedDelta = 0.599;
        double result = bsPricer.FirstOrderDerivative("SpotPrice", 0.01);
        Assert.That(result, Is.EqualTo(expectedDelta).Within(0.01));
    }

    [Test]
    public void TestMonteCarloBSFirstDerivatives(){
        double expectedDelta = 0.599;
        double[] result = mcBsPricer.FirstOrderDerivative("SpotPrice", 1, 100);
        Assert.That(result[0], Is.EqualTo(expectedDelta).Within(0.05));
        Assert.That(result[1]/result[0], Is.LessThan(0.05));
    }

}