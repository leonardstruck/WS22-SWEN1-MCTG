namespace Models.Card.Monsters;

public class Goblin : MonsterCard
{
    
    public Goblin(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // Default state
        return new DefenseResult();
    }

    public override AttackResult Attack(ICard opponent)
    {
        // Goblins are too afraid of Dragons to attack
        if (opponent is Dragon)
        {
            return new AttackResult
            {
                Stunned = true
            };
        }
        
        // Default state
        return new AttackResult();
    }
}