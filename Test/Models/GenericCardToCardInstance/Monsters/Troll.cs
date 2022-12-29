using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class TrollTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalTroll()
    {
        var generic = new GenericCard("Troll", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Troll)));
        });
    }
    
    [Test]
    public void FireTroll()
    {
        var generic = new GenericCard("FireTroll", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Troll)));
        });
    }
    
    [Test]
    public void WaterTroll()
    {
        var generic = new GenericCard("WaterTroll", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Troll)));
        });
    }
}