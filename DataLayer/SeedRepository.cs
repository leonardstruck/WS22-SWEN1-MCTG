using Npgsql;

namespace DataLayer;

public static class SeedRepository
{
    private static  readonly NpgsqlDataSource Db = Connection.GetInstance().DataSource;
    public static async Task SeedDatabase()
    {
        string sql = await File.ReadAllTextAsync("./seed.sql");
        
        var command = Db.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }
}