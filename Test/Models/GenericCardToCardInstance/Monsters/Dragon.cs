using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class DragonTest
{
    private readonly Guid _guid = Guid.NewGuid();
    
    [Test]
    public void NormalDragon()
    {
        GenericCard generic = new GenericCard("Dragon", 10, _guid);
        ICard card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf(typeof(Dragon)));
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
        });
    }

    [Test]
    public void FireDragon()
    {
        GenericCard generic = new GenericCard("FireDragon", 10, _guid);
        ICard card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf(typeof(Dragon)));
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
        });
    }
    
    [Test]
    public void WaterDragon()
    {
        GenericCard generic = new GenericCard("WaterDragon", 10, _guid);
        ICard card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card, Is.TypeOf(typeof(Dragon)));
            Assert.That(card.Element, Is.EqualTo(Element.Water));
        });
    }
}