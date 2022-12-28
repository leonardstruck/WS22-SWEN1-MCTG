using System.Text;
using Models;
using Models.Card;

namespace GameLogic;

public class Battle
{
    private List<ICard> _deck1;
    private List<ICard> _deck2;
    private int _rounds;

    public BattleLog Deck1Log { get; } = new();
    public BattleLog Deck2Log { get; } = new();

    public Battle(ICard[] deck1, ICard[] deck2)
    {
        _deck1 = new List<ICard>(deck1);
        _deck2 = new List<ICard>(deck2);
    }
    
    public ICard[] GetDeck1() => _deck1.ToArray();
    public ICard[] GetDeck2() => _deck2.ToArray();

    public Battle(GenericCard[] deck1, GenericCard[] deck2)
    {
        _deck1 = deck1.Select(card => card.GetCardInstance()).ToList();
        _deck2 = deck2.Select(card => card.GetCardInstance()).ToList();
    }

    public Winner Play()
    {
        do
        {
            ++_rounds; 
            PlayRound();
        } while (!IsGameOver());
        
        // Add the winner of the game to the log
        var winner = GetWinner();

        switch (winner)
        {
            case Winner.Draw:
                Deck1Log.AddEntry($"You drew with your opponent after {_rounds} rounds.");
                Deck2Log.AddEntry($"You drew with your opponent after {_rounds} rounds.");
                break;
            
            case Winner.Player1:
                Deck1Log.AddEntry($"You won the game after {_rounds} rounds.");
                Deck2Log.AddEntry($"You lost the game after {_rounds} rounds.");
                break;
            
            case Winner.Player2:
                Deck1Log.AddEntry($"You lost the game after {_rounds} rounds.");
                Deck2Log.AddEntry($"You won the game after {_rounds} rounds.");
                break;
        }
        
        return winner;
    }


    private void PlayRound()
    {
        var rnd = new Random();
        
        StringBuilder roundLogDeck1 = new();
        StringBuilder roundLogDeck2 = new();
        
        ICard card1 = _deck1[rnd.Next(0, _deck1.Count)];
        ICard card2 = _deck2[rnd.Next(0, _deck2.Count)];
        
        // add cards to log
        roundLogDeck1.Append($"You drew {card1.Name} ({card1.Damage}) and your opponent drew {card2.Name} ({card2.Damage}).").AppendLine();
        roundLogDeck2.Append($"You drew {card2.Name} ({card2.Damage}) and your opponent drew {card1.Name} ({card1.Damage}).").AppendLine();
        
        var round = new BattleRound(card1, card2);
        var winner = round.GetWinner();
        
        // add round result to log
        roundLogDeck1.Append(round.Card1Log).AppendLine();
        roundLogDeck2.Append(round.Card2Log).AppendLine();

        
        switch (winner)
        {
            case Winner.Draw:
            {
                roundLogDeck1.Append("The round was a draw.").AppendLine();
                roundLogDeck2.Append("The round was a draw.").AppendLine();
                break;
            }
            
            case Winner.Player1:
                // remove card2 from deck2 and add it to deck1
                _deck1.Add(card2);
                _deck2.Remove(card2);
                
                roundLogDeck1.Append($"You won the round and gained your opponent's card {card2.Name}").AppendLine();
                roundLogDeck2.Append($"You lost the round and lost your card {card2.Name}").AppendLine();
                break;
            
            case Winner.Player2:
                // remove card1 from deck1 and add it to deck2
                _deck2.Add(card1);
                _deck1.Remove(card1);
                
                roundLogDeck1.Append($"You lost the round and lost your card {card1.Name}").AppendLine();
                roundLogDeck2.Append($"You won the round and gained your opponent's card {card1.Name}").AppendLine();
                break;
        }
        
        // add round log to battle log
        Deck1Log.AddEntry(roundLogDeck1.ToString());
        Deck2Log.AddEntry(roundLogDeck2.ToString());
    }
    private Winner GetWinner()
    {
        if(_deck1.Count == _deck2.Count) return Winner.Draw;
        return _deck1.Count > _deck2.Count ? Winner.Player1 : Winner.Player2;
    }
    
    private bool IsGameOver()
    {
        return _deck1.Count == 0 || _deck2.Count == 0 || _rounds == 100;
    }
    
}