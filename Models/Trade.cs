using Models.Card;

namespace Models;

public class Trade
{
    public Guid? Id { get; }
    public ICard CardToTrade { get; }
    public string Type { get; }
    public int MinDamage { get; }
    
    public Trade(ICard cardToTrade, string type, int minDamage, Guid? id = null)
    {
        Id = id;
        CardToTrade = cardToTrade;
        Type = type;
        MinDamage = minDamage;
    }
}