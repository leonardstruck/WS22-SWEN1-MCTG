namespace Models.Card.Monsters;

public class Wizard : MonsterCard
{
    public Wizard(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // Wizards can control Orks so they are not able to damage them
        if (opponent is Ork)
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