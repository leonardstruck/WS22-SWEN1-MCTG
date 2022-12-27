namespace Models.Card.Monsters;

public class Elf : MonsterCard
{
  
    public Elf(Guid id, string name, int damage, Element elementType) : base(id, name, damage, elementType)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // FireElves know Dragons since they were little and can evade their attacks
        if (opponent is Dragon && this.Element == Element.Fire)
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