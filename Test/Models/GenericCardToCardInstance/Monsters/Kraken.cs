using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class KrakenTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalKraken()
    {
        var generic = new GenericCard("Kraken", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Kraken)));
        });
    }
    
    [Test]
    public void FireKraken()
    {
        var generic = new GenericCard("FireKraken", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Kraken)));
        });
    }
    
    [Test]
    public void WaterKraken()
    {
        var generic = new GenericCard("WaterKraken", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Kraken)));
        });
    }
}