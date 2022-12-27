namespace Models.Card.Monsters;

public class Knight : MonsterCard
{

    
    public Knight(Guid id, int damage, Element elementType) : base(id, "Knight", damage, elementType)
    {
    }
    
    public Knight(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // The armor of Knights is so heavy that WaterSpells make them drown instantly
        if (opponent is SpellCard && opponent.Element == Element.Water)
        {
            return new DefenseResult
            {
                InstantDeath = true
            };
        }
        // default state
        return new DefenseResult();
    }

    public override AttackResult Attack(ICard opponent)
    {
        // default state
        return new AttackResult();
    }
}