using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class GoblinTest
{
    private readonly Guid _id = Guid.NewGuid();
    
    [Test]
    public void NormalGoblin()
    {
        var generic = new GenericCard("Goblin", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Goblin)));
        });
    }
    
    [Test]
    public void FireGoblin()
    {
        var generic = new GenericCard("FireGoblin", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Goblin)));
        });
    }
    
    [Test]
    public void WaterGoblin()
    {
        var generic = new GenericCard("WaterGoblin", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Goblin)));
        });
    }
}