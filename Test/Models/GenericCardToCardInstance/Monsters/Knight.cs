using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class KnightTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalKnight()
    {
        var generic = new GenericCard("Knight", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Knight)));
        });
    }
    
    [Test]
    public void FireKnight()
    {
        var generic = new GenericCard("FireKnight", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Knight)));
        });
    }
    
    [Test]
    public void WaterKnight()
    {
        var generic = new GenericCard("WaterKnight", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Knight)));
        });
    }
}