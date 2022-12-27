namespace Models.Card;

public abstract class SpellCard : ICard
{
    public Guid Id { get; }
    public string Name { get; }
    public int Damage { get; }
    public Element Element { get; }
    
    public abstract DefenseResult Defend(ICard attacker);
    public abstract AttackResult Attack(ICard defender);
    
    protected SpellCard(Guid id, string name, int damage, Element element)
    {
        Id = id;
        Name = name;
        Damage = damage;
        Element = element;
    }
}