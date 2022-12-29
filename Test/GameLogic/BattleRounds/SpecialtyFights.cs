using GameLogic;
using Models;
using Models.Card;
using Models.Card.Monsters;
using Models.Card.Spells;

namespace Test.GameLogic.BattleRounds;

[TestFixture]
public class SpecialtyFights
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void WaterSpellVsKnight()
    {
        var waterSpell = new WaterSpell(_id,10);
        var knight = new Knight(_id, 10, Element.Normal);

        var round = new BattleRound(waterSpell, knight);
        
        Assert.That(round.GetWinner(), Is.EqualTo(Winner.Player1));
    }
}