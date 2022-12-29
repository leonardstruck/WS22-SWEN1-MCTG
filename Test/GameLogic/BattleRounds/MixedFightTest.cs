using GameLogic;
using Models;
using Models.Card;
using Models.Card.Monsters;
using Models.Card.Spells;
namespace Test.GameLogic.BattleRounds;

[TestFixture]
public class MixedFightTest
{
    private readonly Guid _id = Guid.NewGuid();
    
    [Test]
    public void FireSpell10VsWaterGoblin10()
    {
        var fireSpell = new FireSpell(_id, 10);
        var waterGoblin = new Goblin(_id, "WaterGoblin", 10, Element.Water);
        
        var battleRound = new BattleRound(fireSpell, waterGoblin);
        
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Player2));
    }

    [Test]
    public void WaterSpell10VsWaterGoblin10()
    {
        var waterSpell = new WaterSpell(_id, 10);
        var waterGoblin = new Goblin(_id, "WaterGoblin", 10, Element.Water);
        
        var battleRound = new BattleRound(waterSpell, waterGoblin);
        
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Draw));
    }

    [Test]
    public void RegularSpell10VsWaterGoblin10()
    {
        var regularSpell = new RegularSpell(_id, 10);
        var waterGoblin = new Goblin(_id, "WaterGoblin", 10, Element.Water);
        
        var battleRound = new BattleRound(regularSpell, waterGoblin);
        
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Player1));
    }

    [Test]
    public void RegularSpell10VsKnight15()
    {
        var regularSpell = new RegularSpell(_id, 10);
        var knight = new Knight(_id, 15, Element.Normal);
        
        var battleRound = new BattleRound(regularSpell, knight);
        
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Player2));
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Player2));
    }
}