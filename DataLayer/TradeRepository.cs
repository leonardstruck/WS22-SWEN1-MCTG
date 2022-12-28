using Models;

namespace DataLayer;

public class TradeRepository
{
    public static async Task<Trade[]> GetTrades()
    {
        await using var db = Connection.GetDataSource();

        await using var cmd = db.CreateCommand("SELECT id, card_id, type, \"minDamage\" FROM trading");
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        var trades = new List<Trade>();
        
        while (await reader.ReadAsync())
        {
            var card = await CardRepository.GetCardById(reader.GetGuid(1));
            if (card == null)
            {
                throw new Exception("Card not found");
            }
            var type = reader.GetString(2);
            var minDamage = reader.GetInt32(3);
            var id = reader.GetGuid(0);
            trades.Add(new Trade(card.GetCardInstance(), type, minDamage, id));
        }
        
        return trades.ToArray();
    }
}