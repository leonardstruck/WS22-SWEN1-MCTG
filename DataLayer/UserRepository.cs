using System.Security.Cryptography;
using Models;
using Npgsql;
using NpgsqlTypes;

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
    
    public static async Task<string?> LoginUser(Credentials credentials)
    {
        var hashCmd = Db.CreateCommand("SELECT id, password FROM \"user\" WHERE username = @username LIMIT 1");
        hashCmd.Parameters.AddWithValue("username", credentials.Username);
        
        var hashReader = await hashCmd.ExecuteReaderAsync();
        if (!hashReader.HasRows)
            return null;
        
        await hashReader.ReadAsync();
        
        var passwordHash = hashReader["password"].ToString()!;
        var userId = hashReader["id"].ToString()!;
        
        if (!BCrypt.Net.BCrypt.Verify(credentials.Password, passwordHash))
            return null;
        
        // Generate bearer token
        var bytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(bytes);
        
        // Store token in database
        var tokenCmd = Db.CreateCommand("INSERT INTO session (token, user_id) VALUES (@token, @user_id)");
        tokenCmd.Parameters.AddWithValue("token", token);
        tokenCmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, Guid.Parse(userId));
        
        await tokenCmd.ExecuteNonQueryAsync();

        return token;
    }
}