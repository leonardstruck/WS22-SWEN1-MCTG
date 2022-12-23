namespace DataLayer;

public static class SeedRepository
{ 
    public static async Task SeedDatabase()
    {
        await using var db = Connection.GetDataSource();
        string sql = await File.ReadAllTextAsync("seed.sql");
        
        await using var command = db.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }
}