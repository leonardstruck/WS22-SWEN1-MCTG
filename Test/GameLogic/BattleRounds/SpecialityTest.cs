using Models.Card;
using Models.Card.Monsters;
using Models.Card.Spells;

namespace Test.GameLogic.BattleRounds;

[TestFixture]
public class SpecialtyTest
{
    private readonly Guid _id = Guid.NewGuid();
    [Test]
    public void GoblinsAreAfraidOfDragons()
    {
        var goblin = new Goblin(_id, "Afraid", 10, Element.Normal);
        var dragon = new Dragon(_id, "Dragobert", damage: 1, Element.Fire);

        var goblinAttack = goblin.Attack(dragon);
        
        Assert.That(goblinAttack.Stunned, Is.True);
    }

    [Test]
    public void OrksAreNotAbleToAttackWizards()
    {
        var ork = new Ork(_id, "Orkana", 10, Element.Normal);
        var wizard = new Wizard(_id,"Istar", 9, Element.Normal);

        var wizardDefense = wizard.Defend(ork);
        
        Assert.That(wizardDefense.Immune, Is.True);
    }

    [Test]
    public void KnightsDrownImmediatelyOnWaterSpellAttack()
    {
        var knight = new Knight(_id, 10, Element.Fire);
        var waterSpell = new WaterSpell(_id, 20);

        var knightDefense = knight.Defend(waterSpell);

        Assert.That(knightDefense.InstantDeath, Is.True);
    }

    [Test]
    public void KrakenAreImmuneAgainstSpells()
    {
        var kraken = new Kraken(_id, "Gundula", 20, Element.Water);
        var waterSpell = new WaterSpell(_id, 20);

        var krakenDefense = kraken.Defend(waterSpell);

        Assert.That(krakenDefense.Immune, Is.True);
    }

    [Test]
    public void FireElvesEvadeDragonAttacks()
    {
        var fireElf = new Elf(_id,"FireElf", 20, Element.Fire);
        var dragon = new Dragon(_id, "Dragobert", 10, Element.Fire);

        var fireElfDefense = fireElf.Defend(dragon);

        Assert.That(fireElfDefense.Immune, Is.True);
    }
}