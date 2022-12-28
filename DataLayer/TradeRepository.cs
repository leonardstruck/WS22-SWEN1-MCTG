using Models;
using Models.Card;

namespace DataLayer;

public class TradeRepository
{
    public static async Task<Trade?> GetTrade(Guid tradeId)
    {
        await using var db = Connection.GetDataSource();
        await using var cmd = db.CreateCommand("SELECT id, card_id, type, \"minDamage\" FROM trading");
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;

        var id = reader.GetGuid(0);
        var cardId = reader.GetGuid(1);
        var type = reader.GetString(2);
        var minDamage = reader.GetInt32(3);

        return new Trade(cardId, type, minDamage, id);
    }
    
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
    
    public static async Task<Guid?> GetTradeOwner(Guid tradeId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("SELECT owner_id FROM trading INNER JOIN card c on c.id = trading.card_id WHERE trading.id = @trade_id");
        cmd.Parameters.AddWithValue("@trade_id", tradeId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return reader.GetGuid(0);
        }
        
        return null;
    }
    
    public static async Task DeleteTrade(Guid tradeId)
    {
        await using var db = Connection.GetDataSource();
        
        // delete trade entry and unblock card
        await using var cmd = db.CreateCommand("UPDATE card SET \"tradeLock\" = false " +
                                               "WHERE id = (SELECT card_id FROM trading WHERE id = @trade_id);" +
                                               "DELETE FROM trading WHERE id = @trade_id;");
        cmd.Parameters.AddWithValue("@trade_id", tradeId);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<bool> CheckIfOfferedCardMatchesCriteria(Guid tradeId, Guid cardId)
    {
        var trade = await GetTrade(tradeId);
        if (trade == null)
            return false;

        var card = await CardRepository.GetCardById(cardId);
        if (card == null)
            return false;

        var cardInstance = card.GetCardInstance();

        switch (trade.Type)
        {
            case "monster":
                // check if card implements monstercard interface
                if (cardInstance is MonsterCard monsterCard)
                {
                    return monsterCard.Damage >= trade.MinimumDamage;
                }
                break;
            case "spell":
                if (cardInstance is SpellCard spellCard)
                {
                    return spellCard.Damage >= trade.MinimumDamage;
                }
                break;
        }
        
        return false;
    }

    public static async Task CarryOutTrade(Guid tradeId, Guid tradedCard)
    {
        var trade = await GetTrade(tradeId);
        if (trade == null)
            throw new Exception("Trade not found");

        var tradeOfferCardOwner = await CardRepository.GetCardOwner(trade.CardToTrade);
        var cardOwner = await CardRepository.GetCardOwner(tradedCard);
        
        if(tradeOfferCardOwner == null || cardOwner == null)
            throw new Exception("Card owner not found");


        // switch owner id's of cards and remove tradeLock and trade entry
        await using var db = Connection.GetDataSource();
        await using var cmd = db.CreateCommand("UPDATE card SET owner_id = @new_owner_id, \"tradeLock\" = false WHERE id = @card_id;" +
                                               "UPDATE card SET owner_id = @old_owner_id, \"tradeLock\" = false WHERE id = @trade_card_id;" +
                                               "DELETE FROM trading WHERE id = @trade_id;");
        
        cmd.Parameters.AddWithValue("@new_owner_id", tradeOfferCardOwner);
        cmd.Parameters.AddWithValue("@old_owner_id", cardOwner);
        cmd.Parameters.AddWithValue("@card_id", tradedCard);
        cmd.Parameters.AddWithValue("@trade_card_id", trade.CardToTrade);
        cmd.Parameters.AddWithValue("@trade_id", tradeId);
        
        await cmd.ExecuteNonQueryAsync();
    }
}