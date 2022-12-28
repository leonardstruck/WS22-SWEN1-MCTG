using Models.Card;

namespace DataLayer;

public static class CardRepository
{
    public static async Task<Guid> CreateCard(GenericCard card, Guid packageId)
    {
        await using var db = Connection.GetDataSource();

        await using var command = db.CreateCommand("INSERT INTO card (name, damage, package_id) VALUES (@name, @damage, @package_id) RETURNING id");
        command.Parameters.AddWithValue("name", card.Name);
        command.Parameters.AddWithValue("damage", card.Damage);
        command.Parameters.AddWithValue("package_id", packageId);

        if(await command.ExecuteScalarAsync() is not Guid id)
            throw new Exception("Failed to create card");

        return id;
    }

    public static async Task<GenericCard[]> GetCardsByOwner(Guid ownerId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var command = db.CreateCommand("SELECT id, name, damage FROM card WHERE owner_id = @owner_id");
        command.Parameters.AddWithValue("owner_id", ownerId);
        
        await using var reader = await command.ExecuteReaderAsync();
        var cards = new List<GenericCard>();
        
        while(await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var name = reader.GetString(1);
            var damage = reader.GetInt32(2);

            var card = new GenericCard(name, damage, id);
            
            cards.Add(card);
        }
        
        return cards.ToArray();
    }

    public static async Task<GenericCard[]> GetCardsInDeckByOwner(Guid ownerId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var command = db.CreateCommand("SELECT id, name, damage FROM card WHERE owner_id = @owner_id AND  \"inDeck\" IS TRUE AND \"tradeLock\" IS FALSE");
        command.Parameters.AddWithValue("owner_id", ownerId);
        
        await using var reader = await command.ExecuteReaderAsync();
        var cards = new List<GenericCard>();
        
        while(await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var name = reader.GetString(1);
            var damage = reader.GetInt32(2);

            var card = new GenericCard(name, damage, id);
            
            cards.Add(card);
        }
        
        return cards.ToArray();
    }

    public static async Task<bool> PutCardsInDeck(Guid ownerId, Guid[] cardIds)
    {
        await using var db = Connection.GetDataSource();
        
        // check if 4 cards were provided
        if(cardIds.Length != 4)
            return false;
        
        // check if all cards belong to the user and are not locked because they are traded
        await using var countOwnedCommand = db.CreateCommand("SELECT COUNT(*) FROM card WHERE owner_id = @owner_id AND id = ANY(@card_ids) AND \"tradeLock\" IS FALSE");
        countOwnedCommand.Parameters.AddWithValue("owner_id", ownerId);
        countOwnedCommand.Parameters.AddWithValue("card_ids", cardIds);
        
        var countOwned = (long) (await countOwnedCommand.ExecuteScalarAsync() ?? 0);
        if(countOwned != 4)
            return false;
        
        
        // reset all cards in deck
        await using var resetCommand = db.CreateCommand("UPDATE card SET \"inDeck\" = FALSE WHERE owner_id = @owner_id");
        resetCommand.Parameters.AddWithValue("owner_id", ownerId);
        await resetCommand.ExecuteNonQueryAsync();
        
        // put cards in deck
        await using var command2 = db.CreateCommand("UPDATE card SET \"inDeck\" = TRUE WHERE id = ANY(@card_ids) AND owner_id = @owner_id");
        command2.Parameters.AddWithValue("card_ids", cardIds);
        command2.Parameters.AddWithValue("owner_id", ownerId);
        
        await command2.ExecuteNonQueryAsync();
        return true;
    }
    
    public static async Task UpdateDeck(Guid ownerId, Guid[] cardIds)
    {
        await using var db = Connection.GetDataSource();
        
        // set owner id for every card
        await using var command = db.CreateCommand("UPDATE card SET owner_id = @owner_id, \"inDeck\" = false WHERE id = ANY(@card_ids) AND \"tradeLock\" IS FALSE");
        command.Parameters.AddWithValue("owner_id", ownerId);
        command.Parameters.AddWithValue("card_ids", cardIds);

        await command.ExecuteNonQueryAsync();
    }
}