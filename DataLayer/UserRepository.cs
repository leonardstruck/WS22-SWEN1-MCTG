using System.Security.Cryptography;
using Models;
using NpgsqlTypes;

namespace DataLayer;

public static class UserRepository
{
    public static async Task<User?> RegisterUser(Credentials credentials)
    {
        await using var db = Connection.GetDataSource();

        // Check if user already exists
        var user = await GetUserByUsername(credentials.Username);
        if (user != null)
            return null;
        
        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
        
        // Create user

        var newUser = new User(credentials.Username);
        
        await using var cmd = db.CreateCommand("INSERT INTO \"user\" (username, password, coins) VALUES (@username, @password, @coins) RETURNING id");
        cmd.Parameters.AddWithValue("username", credentials.Username);
        cmd.Parameters.AddWithValue("password", passwordHash);
        cmd.Parameters.AddWithValue("coins", newUser.Coins);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (reader.Read())
        {
            newUser.Id = reader.GetGuid(0);
        }
        
        return newUser;
    }
    
    public static async Task<User?> GetUserByUsername(string username)
    {
        await using var db = Connection.GetDataSource();

        await using var cmd = db.CreateCommand("SELECT id, username, password, coins, name, bio, image FROM \"user\" WHERE username = @username LIMIT 1");
        cmd.Parameters.AddWithValue("username", username);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return new User(username)
        {
            Id = reader.GetGuid(0),
            Username = reader["username"].ToString()!,
            
            Name = reader["name"].ToString(),
            Bio = reader["bio"].ToString(),
            Image = reader["image"].ToString(),
            Coins = int.Parse(reader["coins"].ToString()!),
        };
    }
    
    public static async Task<string?> LoginUser(Credentials credentials)
    {
        await using var db = Connection.GetDataSource();

        await using var hashCmd = db.CreateCommand("SELECT id, password FROM \"user\" WHERE username = @username LIMIT 1");
        hashCmd.Parameters.AddWithValue("username", credentials.Username);
        
        await using var hashReader = await hashCmd.ExecuteReaderAsync();
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
        await using var tokenCmd = db.CreateCommand("INSERT INTO session (token, user_id) VALUES (@token, @user_id)");
        tokenCmd.Parameters.AddWithValue("token", token);
        tokenCmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, Guid.Parse(userId));
        
        await tokenCmd.ExecuteNonQueryAsync();

        return token;
    }
    
    public static async Task<User?> GetUserByToken(string token)
    {
        await using var db = Connection.GetDataSource();

        await using var cmd = db.CreateCommand(
            "SELECT id, username, password, coins, name, bio, image FROM \"user\" " +
            "WHERE id = (SELECT user_id FROM session WHERE token = @token AND expires > now() LIMIT 1) LIMIT 1");
        cmd.Parameters.AddWithValue("token", token);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return new User(reader["username"].ToString()!)
        {
            Id = reader.GetGuid(0),
            Username = reader["username"].ToString()!,
            
            Bio = reader["bio"].ToString(),
            Image = reader["image"].ToString(),
            Coins = int.Parse(reader["coins"].ToString()!),
        };
    }

    public static async Task<bool> SpendCoins(Guid userId, int amount)
    {
        await using var db = Connection.GetDataSource();

        // Check if user has enough coins
        await using var cmd = db.CreateCommand("SELECT coins FROM \"user\" WHERE id = @id LIMIT 1");
        cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, userId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows)
            return false;
        
        await reader.ReadAsync();
        
        var coins = int.Parse(reader["coins"].ToString()!);
        if (coins < amount)
            return false;
        
        // Update user coins
        await using var updateCmd = db.CreateCommand("UPDATE \"user\" SET coins = coins - @amount WHERE id = @id");
        updateCmd.Parameters.AddWithValue("amount", amount);
        updateCmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, userId);
        
        return await updateCmd.ExecuteNonQueryAsync() == 1;
    }
    
    public static async Task<bool> RefundCoins(Guid userId, int amount)
    {
        await using var db = Connection.GetDataSource();

        await using var cmd = db.CreateCommand("UPDATE \"user\" SET coins = coins + @amount WHERE id = @id");
        cmd.Parameters.AddWithValue("amount", amount);
        cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, userId);
        
        return await cmd.ExecuteNonQueryAsync() == 1;
    }
}