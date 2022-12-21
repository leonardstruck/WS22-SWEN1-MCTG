using Models;
using Npgsql;

namespace DataLayer;

public class UserRepository
{
    private static readonly NpgsqlDataSource Db = Connection.GetInstance().DataSource;

    public static async Task<User?> RegisterUser(Credentials credentials)
    {
        // Check if user already exists
        var user = await GetUserByUsername(credentials.Username);
        if (user != null)
            return null;
        
        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
        
        // Create user

        var newUser = new User(credentials.Username);
        
        var cmd = Db.CreateCommand("INSERT INTO \"user\" (username, password, coins) VALUES (@username, @password, @coins) RETURNING id");
        cmd.Parameters.AddWithValue("username", credentials.Username);
        cmd.Parameters.AddWithValue("password", passwordHash);
        cmd.Parameters.AddWithValue("coins", newUser.Coins);
        
        var reader = await cmd.ExecuteReaderAsync();
        
        if (reader.Read())
        {
            newUser.Id = reader["id"].ToString();
        }
        
        return newUser;
    }
    
    public static async Task<User?> GetUserByUsername(string username)
    {
        var cmd = Db.CreateCommand("SELECT id, username, password, coins, name, bio, image FROM \"user\" WHERE username = @username LIMIT 1");
        cmd.Parameters.AddWithValue("username", username);
        
        var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return new User(username)
        {
            Id = reader["id"].ToString(),
            Username = reader["username"].ToString()!,
            
            Bio = reader["bio"].ToString(),
            Image = reader["image"].ToString(),
            Coins = int.Parse(reader["coins"].ToString()!),
        };
    }
}