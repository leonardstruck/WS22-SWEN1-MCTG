using Models;
using Npgsql;

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
        
        await using var command = db.CreateCommand("SELECT id, name, damage FROM card WHERE owner_id = @owner_id AND  \"inDeck\" IS TRUE");
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
}