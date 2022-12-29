using Models.Card;
using Models.Card.Monsters;
namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class WizardTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalWizard()
    {
        var generic = new GenericCard("Wizard", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Normal));
            Assert.That(card, Is.TypeOf(typeof(Wizard)));
        });
    }
    
    [Test]
    public void FireWizard()
    {
        var generic = new GenericCard("FireWizard", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Fire));
            Assert.That(card, Is.TypeOf(typeof(Wizard)));
        });
    }
    
    [Test]
    public void WaterWizard()
    {
        var generic = new GenericCard("WaterWizard", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element, Is.EqualTo(Element.Water));
            Assert.That(card, Is.TypeOf(typeof(Wizard)));
        });
    }
}