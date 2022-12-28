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
            var cardId = reader.GetGuid(1);
            var type = reader.GetString(2);
            var minDamage = reader.GetInt32(3);
            var id = reader.GetGuid(0);
            trades.Add(new Trade(cardId, type, minDamage, id));
        }
        
        return trades.ToArray();
    }
    
    public static async Task AddTrade(Trade trade)
    {
        await using var db = Connection.GetDataSource();
        
        // insert into trading table and update tradeLock in card table
        await using var cmd = db.CreateCommand("INSERT INTO trading (card_id, type, \"minDamage\") VALUES (@card_id, @type, @minDamage); UPDATE card SET \"tradeLock\" = true WHERE id = @card_id;");
        cmd.Parameters.AddWithValue("@card_id", trade.CardToTrade);
        cmd.Parameters.AddWithValue("@type", trade.Type);
        cmd.Parameters.AddWithValue("@minDamage", trade.MinimumDamage);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public static async Task<bool> CheckIfTradeExists(Guid cardId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("SELECT id FROM trading WHERE card_id = @card_id");
        cmd.Parameters.AddWithValue("@card_id", cardId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        return await reader.ReadAsync();
    }
}