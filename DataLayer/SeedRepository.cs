using Npgsql;

namespace DataLayer;

public class SeedRepository
{
    private readonly NpgsqlDataSource _db = Connection.GetInstance().DataSource;
    public async Task Write()
    {
        string sql = await System.IO.File.ReadAllTextAsync("./seed.sql");
        
        var command = _db.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }
}