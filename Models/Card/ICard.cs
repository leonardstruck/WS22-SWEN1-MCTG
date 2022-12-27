namespace Models.Card;

public interface ICard
{
    Guid Id { get; }
    string Name { get; }

    int Damage { get; }
    Element Element { get; }
    DefenseResult Defend(ICard attacker);
    AttackResult Attack(ICard defender);
}