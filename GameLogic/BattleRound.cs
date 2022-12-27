using System.Text;
using Models;
using Models.Card;

namespace GameLogic;

public class BattleRound
{
    private readonly ICard _card1;
    private readonly ICard _card2;
    
    private StringBuilder _battleLogCard1 = new StringBuilder();
    private StringBuilder _battleLogCard2 = new StringBuilder();
    
    public string Card1Log => _battleLogCard1.ToString();
    public string Card2Log => _battleLogCard2.ToString();

    public BattleRound(ICard card1, ICard card2)
    {
        _card1 = card1;
        _card2 = card2;
    }
    
    public Winner GetWinner()
    {
        int damage1 = HandleAttack(_card1, _card2);
        int damage2 = HandleAttack(_card2, _card1);
        
        if (damage1 > damage2)
            return Winner.Player1;
        
        if (damage2 > damage1)
            return Winner.Player2;
        
        return Winner.Draw;
    }
    
    private int HandleAttack(ICard attacker, ICard defender)
    {
        StringBuilder attackerLog = new();
        StringBuilder defenderLog = new();
        
        int damage = 0;
        AttackResult attack = attacker.Attack(defender);

        if (!attack.Stunned)
        {
            damage = attacker.Damage;
        }

        DefenseResult defense = defender.Defend(attacker);

        if (defense.Immune)
        {
            damage = 0;
        }

        if (defense.InstantDeath)
        {
            damage = int.MaxValue;
        }

        int resultingDamage = (int)Math.Round(damage * attack.Factor);

        if (resultingDamage > 0 && !defense.InstantDeath)
        {
            attackerLog.Append($"{attacker.Name} dealt {resultingDamage} (factor: {attack.Factor}x) damage to {defender.Name}.").AppendLine();
            defenderLog.Append($"{defender.Name} took {resultingDamage} (factor: {attack.Factor}x) damage from {attacker.Name}.").AppendLine();
        } else if (defense.InstantDeath)
        {
            attackerLog.Append($"{attacker.Name}'s attack killed {defender.Name} immediately.").AppendLine();
            defenderLog.Append($"{defender.Name} was killed by {attacker.Name} immediately.").AppendLine();
        } else if (defense.Immune)
        {
            attackerLog.Append($"{attacker.Name}'s attack was blocked by {defender.Name}.").AppendLine();
            defenderLog.Append($"{defender.Name} blocked {attacker.Name}'s attack.").AppendLine();
        } else if (attack.Stunned)
        {
            attackerLog.Append($"{attacker.Name} was stunned and couldn't attack.").AppendLine();
            defenderLog.Append($"{defender.Name} stunned {attacker.Name} and prevented an attack.").AppendLine();
        }

        if (attacker == _card1)
        {
            _battleLogCard1.Append(attackerLog);
            _battleLogCard2.Append(defenderLog);
        } else
        {
            _battleLogCard1.Append(defenderLog);
            _battleLogCard2.Append(attackerLog);
        }
        return resultingDamage;
    }
}