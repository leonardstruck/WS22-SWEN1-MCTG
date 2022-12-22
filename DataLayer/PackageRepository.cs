using Models;
using Npgsql;

namespace DataLayer;

public static class PackageRepository
{
    private static readonly NpgsqlDataSource Db = Connection.GetInstance().DataSource;

    public static async Task CreatePackage(GenericCard[] cards)
    {
        // Check if package contains 5 cards
        if (cards.Length != 5)
        {
            throw new Exception("Package must contain 5 cards");
        }
        
        // Create Package in DB and get ID
        var packageCmd = Db.CreateCommand("INSERT INTO package DEFAULT VALUES RETURNING id");

        if(await packageCmd.ExecuteScalarAsync() is not Guid packageId)
        {
            throw new Exception("Failed to create package");
        }

        foreach (var card in cards)
        {
            await CardRepository.CreateCard(card, packageId);
        }
    }
    
    public static async Task<GenericCard[]?> GetCards(Guid packageId)
    {
        var packageCmd = Db.CreateCommand("SELECT id, name, damage FROM card WHERE package_id = @id");
        packageCmd.Parameters.AddWithValue("id", packageId);
        
        var cards = new List<GenericCard>();

        await using var reader = await packageCmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var name = reader.GetString(1);
            var damage = reader.GetInt32(2);
            
            var card = new GenericCard(name, damage, id);
            cards.Add(card);
        }

        // return null if package is empty (no cards / package not found)
        return cards.Count != 5 ? null : cards.ToArray();
    }

    private static async Task UpdateOwnerRecords(Guid packageId)
    {
        var cmd = Db.CreateCommand("UPDATE card SET owner_id = (SELECT acquired_by FROM package WHERE id = @package_id) WHERE package_id = @package_id");
        cmd.Parameters.AddWithValue("package_id", packageId);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<Package?> GetRandomPackage(Guid userId)
    {
        // update acquiredBy of random package and return it's id
        var packageCmd = Db.CreateCommand("UPDATE package SET acquired_by = @userId WHERE id = (SELECT id FROM package WHERE package.acquired_by IS NULL ORDER BY random() LIMIT 1) RETURNING id");
        packageCmd.Parameters.AddWithValue("userId", userId);
        
        if (await packageCmd.ExecuteScalarAsync() is not Guid packageId)
        {
            return null;
        }
        
        await UpdateOwnerRecords(packageId);
        
        if (await GetCards(packageId) is not {} cards)
        {
            return null;
        }

        var package = new Package(cards)
        {
            Id = packageId
        };
        return package;
    } 
}