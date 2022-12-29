using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class OrkTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalOrk()
    {
        var generic = new GenericCard("Ork", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Ork)));
        });
    }
    
    [Test]
    public void FireOrk()
    {
        var generic = new GenericCard("FireOrk", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Ork)));
        });
    }
    
    [Test]
    public void WaterOrk()
    {
        var generic = new GenericCard("WaterOrk", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Ork)));
        });
    }
}