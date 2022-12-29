using Models.Card;
using Models.Card.Monsters;

namespace Test.Models.GenericCardToCardInstance.Monsters;

[TestFixture]
public class ElfTest
{
    private readonly Guid _id = Guid.NewGuid();

    [Test]
    public void NormalElf()
    {
        var generic = new GenericCard("Elf", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element == Element.Normal);
            Assert.That(card, Is.TypeOf(typeof(Elf)));
        });
    }

    [Test]
    public void FireElf()
    {
        var generic = new GenericCard("FireElf", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element == Element.Fire);
            Assert.That(card, Is.TypeOf(typeof(Elf)));
        });
    }
    
    [Test]
    public void WaterElf()
    {
        var generic = new GenericCard("WaterElf", 10, _id);
        var card = generic.GetCardInstance();
        
        Assert.Multiple(() =>
        {
            Assert.That(card.Element == Element.Water);
            Assert.That(card, Is.TypeOf(typeof(Elf)));
        });
    }
}