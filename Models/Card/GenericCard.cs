using System.Reflection;
using Models.Card;
using Models.Card.Spells;

namespace Models;

public class GenericCard
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public int Damage { get; set; }
    
    public GenericCard(string name, int damage, Guid? id)
    {
        Name = name;
        Damage = damage;
        Id = id;
    }

    public ICard GetCardInstance()
    {
        if (Id == null)
        {
            throw new Exception("Id is null");
        }
        
        var cardId = (Guid) Id;
        
        // Check if the card is a spell card
        if (Name.Contains("Spell"))
        {
            switch (Name)
            {
                case "WaterSpell":
                    return new WaterSpell(cardId, Damage);
                case "FireSpell":
                    return new FireSpell(cardId, Damage);
                default:
                    return new RegularSpell(cardId, Damage);
            }
        }
        
        // Since the card is not a spell, it must be a monster card
        var type = typeof(MonsterCard);
        
        // Get type that inherits from MonsterCard and contains the name of the card
        var monsterCard = type.Assembly.GetTypes().FirstOrDefault(t => Name.Contains(t.Name));
        
        if (monsterCard == null)
        {
            throw new ArgumentException($"Failed parsing monster card {Name}");
        }

        var element = GetElement(monsterCard);

        // Create instance of the monster card
        var obj = Activator.CreateInstance(monsterCard, new object[] { Id = cardId, Name, Damage, element });
        if(obj is not MonsterCard parsedCard)
        {
            throw new ArgumentException($"Failed parsing monster card {Name}");
        }
        
        return parsedCard;
    }
    
    private Element GetElement(MemberInfo type)
    {
        if (type.Name == Name)
        {
            return Element.Normal;
        }
        var element = Name.Replace(type.Name, "");
        return (Element) Enum.Parse(typeof(Element), element);
    }
}