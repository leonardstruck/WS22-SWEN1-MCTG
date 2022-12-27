namespace Models.Card.Spells;

public class RegularSpell : SpellCard
{
    public RegularSpell(Guid id, int damage) : base(id, "RegularSpell", damage, Element.Normal)
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
                Factor = 2
            },
            // default state
            _ => new AttackResult()
        };
    }
}