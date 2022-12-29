using Models.Card;

namespace Test.Models.GenericCardToCardInstance;

[TestFixture]
public class SpellsTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void RegularSpell()
    {
        var generic = new GenericCard("RegularSpell", 10, _id);
        var card = generic.GetCardInstance();

        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf<global::Models.Card.Spells.RegularSpell>());
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
        });
    }

    [Test]
    public void FireSpell()
    {
        var generic = new GenericCard("FireSpell", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf<global::Models.Card.Spells.FireSpell>());
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
        });
    }
    
    [Test]
    public void WaterSpell()
    {
        var generic = new GenericCard("WaterSpell", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf<global::Models.Card.Spells.WaterSpell>());
            Assert.That(card.Element, Is.EqualTo(Element.Water));
        });
    }
}