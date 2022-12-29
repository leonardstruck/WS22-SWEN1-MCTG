using GameLogic;
using Models;
using Models.Card.Spells;

namespace Test.GameLogic.BattleRounds;

[TestFixture]
public class SpellFights
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void FireSpell10VsWaterSpell20()
    {
        var fireSpell = new FireSpell(_id, 10);
        var waterSpell = new WaterSpell(_id, 20);

        var round = new BattleRound(fireSpell, waterSpell);
        
        Assert.That(round.GetWinner(), Is.EqualTo(Winner.Player2));
    }

    [Test]
    public void FireSpell20VsWaterSpell5()
    {
        var fireSpell = new FireSpell(_id, 20);
        var waterSpell = new WaterSpell(_id, 5);

        var round = new BattleRound(fireSpell, waterSpell);

        Assert.That(round.GetWinner(), Is.EqualTo(Winner.Draw));
    }

    [Test]
    public void FireSpell90VsWaterSpell5()
    {
        var fireSpell = new FireSpell(_id, 90);
        var waterSpell = new WaterSpell(_id, 5);
        
        var round = new BattleRound(fireSpell, waterSpell);
        
        Assert.That(round.GetWinner(), Is.EqualTo(Winner.Player1));
    }
}