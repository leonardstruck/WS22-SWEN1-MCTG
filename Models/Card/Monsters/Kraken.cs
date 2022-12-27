namespace Models.Card.Monsters;

public class Kraken : MonsterCard
{

    
    public Kraken(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // The Kraken is immune against spells
        if (opponent is SpellCard)
        {
            return new DefenseResult
            {
                Immune = true
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