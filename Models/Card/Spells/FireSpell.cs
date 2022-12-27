namespace Models.Card.Spells;

public class FireSpell : SpellCard
{
    public FireSpell(Guid id, int damage) : base(id, "FireSpell", damage, Element.Fire)
    {
    }

    public override DefenseResult Defend(ICard opponent)
    {
        // default state
        return new DefenseResult();
    }

    public override AttackResult Attack(ICard opponent)
    {
        return opponent.Element switch
        {
            Element.Water => new AttackResult
            {
                Factor = 0.5
            },
            Element.Normal => new AttackResult
            {
                Factor = 2
            },
            // default state
            _ => new AttackResult()
        };
    }
}