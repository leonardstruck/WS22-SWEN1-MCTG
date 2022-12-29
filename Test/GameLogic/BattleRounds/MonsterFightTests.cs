using GameLogic;
using Models;
using Models.Card;
using Models.Card.Monsters;

namespace Test.GameLogic.BattleRounds;

[TestFixture]
public class MonsterFightTests
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void WaterGoblinVsFireTroll()
    {
        var waterGoblin = new Goblin(_id, "WaterGoblin", 10, Element.Water);
        var fireTroll = new Troll(_id, "FireTroll", 15, Element.Fire);

        var battleRound = new BattleRound(waterGoblin, fireTroll);
        Assert.That(battleRound.GetWinner(), Is.EqualTo(Winner.Player2));
    }

    [Test]
    public void FireTrollVsWaterGoblin()
    {
        var waterGoblin = new Goblin(_id, "WaterGoblin", 10, Element.Water);
        var fireTroll = new Troll(_id, "FireTroll", 15, Element.Fire);

        var round = new BattleRound(fireTroll, waterGoblin);
        Assert.That(round.GetWinner(), Is.EqualTo(Winner.Player1));
    }
}