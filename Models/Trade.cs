namespace Models;

public class Trade
{
    public Guid? Id { get; }
    public Guid CardToTrade { get; }
    public string Type { get; }
    public int MinimumDamage { get; }
    
    public Trade(Guid cardToTrade, string type, int minimumDamage, Guid? id = null)
    {
        Id = id;
        CardToTrade = cardToTrade;
        Type = type;
        MinimumDamage = minimumDamage;
    }
}