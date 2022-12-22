using Models;
using Npgsql;

namespace DataLayer;

public static class CardRepository
{
    private static readonly NpgsqlDataSource Db = Connection.GetInstance().DataSource;

    public static async Task<Guid> CreateCard(GenericCard card, Guid packageId)
    {
        var command = Db.CreateCommand("INSERT INTO card (name, damage, package_id) VALUES (@name, @damage, @package_id) RETURNING id");
        command.Parameters.AddWithValue("name", card.Name);
        command.Parameters.AddWithValue("damage", card.Damage);
        command.Parameters.AddWithValue("package_id", packageId);

        if(await command.ExecuteScalarAsync() is not Guid id)
            throw new Exception("Failed to create card");

        return id;
    }
}