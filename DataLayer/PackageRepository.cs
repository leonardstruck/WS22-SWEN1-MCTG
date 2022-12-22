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
}