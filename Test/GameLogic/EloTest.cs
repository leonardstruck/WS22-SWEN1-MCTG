using GameLogic;

namespace Test.GameLogic;

[TestFixture]
public class EloTest
{
    private Elo _elo = null!;

    [SetUp]
    public void Init()
    {
        _elo = new Elo();
    }

    [Test]
    public void CheckDefaultElo()
    {
        Assert.That(_elo.Rating, Is.EqualTo(1500));
    }
    
    [Test]
    public void WinShouldIncreaseRating()
    {
        _elo.UpdateScore(new Elo(), Elo.Win);
        Assert.That(_elo.Rating, Is.GreaterThan(1500));
    }
    
    [Test]
    public void DefeatShouldDecreaseRating()
    {
        _elo.UpdateScore(new Elo(), Elo.Loss);
        Assert.That(_elo.Rating, Is.LessThan(1500));
    }
    
    [Test]
    public void DrawWithHigherRatingShouldDecreaseRating()
    {
        _elo.UpdateScore(new Elo(1400), Elo.Draw);
        Assert.That(_elo.Rating, Is.LessThan(1500));
    }
    
    [Test]
    public void DrawWithLowerRatingShouldIncreaseRating()
    {
        _elo.UpdateScore(new Elo(1600), Elo.Draw);
        Assert.That(_elo.Rating, Is.GreaterThan(1500));
    }
}