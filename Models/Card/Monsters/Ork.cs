namespace Models.Card.Monsters;

public class Ork : MonsterCard
{
    
    public Ork(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // default state
        return new DefenseResult();
    }

    public override AttackResult Attack(ICard opponent)
    {
        // default state
        return new AttackResult();
    }
}