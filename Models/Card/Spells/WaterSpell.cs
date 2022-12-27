namespace Models.Card.Spells;

public class WaterSpell : SpellCard
{
    public WaterSpell(Guid id, int damage) : base(id, "WaterSpell", damage, Element.Water)
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
            Element.Fire => new AttackResult { Factor = 2 },
            Element.Normal => new AttackResult { Factor = 2 },
            // default state
            _ => new AttackResult()
        };
    }
}