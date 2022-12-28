using System.Text.Json.Serialization;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataLayer;

public class BattleRepository
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    public static async Task<Stats?> GetStatsByUserId(Guid userId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("SELECT name, elo, wins, losses FROM \"user\" WHERE id = @user_id LIMIT 1");
        cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return new Stats (
            name: reader.GetString(0),
            elo: reader.GetInt32(1), 
            wins: reader.GetInt32(2), 
            losses: reader.GetInt32(3)
            );
    }

    public static async Task<Stats[]> GetStatsOrderedByElo()
    {
        await using var db = Connection.GetDataSource();
        
        // get all users (excluding admin) ordered by elo and limit to 10
        await using var cmd = db.CreateCommand("SELECT name, elo, wins, losses FROM \"user\" WHERE username != 'admin' ORDER BY elo DESC LIMIT 10");
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        var stats = new List<Stats>();
        
        while (await reader.ReadAsync())
        {
            stats.Add(new Stats(
                name: reader.GetString(0),
                elo: reader.GetInt32(1),
                wins: reader.GetInt32(2),
                losses: reader.GetInt32(3)
            ));
        }
        
        return stats.ToArray();
    }
    
    public static async Task<Guid?> GetOpponent(Guid userId)
    {
        await using var db = Connection.GetDataSource();
        
        // get opponent id that is not the same as the user id, has a lobby_entry timestamp and limit to 1
        // sort by lobby_entry timestamp to get the oldest entry
        await using var cmd = db.CreateCommand("SELECT id FROM \"user\" " +
                                               "WHERE id != @user_id AND lobby_entry IS NOT NULL " +
                                               "ORDER BY lobby_entry ASC LIMIT 1");
        
        cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return reader.GetGuid(0);
    }
    
    public static async Task EnterLobby(Guid userId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("UPDATE \"user\" SET lobby_entry = CURRENT_TIMESTAMP WHERE id = @user_id");
        cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
        
        await cmd.ExecuteNonQueryAsync();
    }
    
    public static async Task LeaveLobby(Guid userId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("UPDATE \"user\" SET lobby_entry = NULL WHERE id = @user_id");
        cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
        
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<Tuple<Guid, bool>> CheckIfBattleExists(Guid opponent1, Guid opponent2)
    {
        await _semaphore.WaitAsync();
        await using var db = Connection.GetDataSource();
        
        // check if a battle exists with the given opponents where timestamp is null
        await using var cmd = db.CreateCommand("SELECT id FROM battle " +
                                               "WHERE ((opponent_1 = @opponent1 AND opponent_2 = @opponent2) " +
                                               "OR (opponent_1 = @opponent2 AND opponent_2 = @opponent1)) " +
                                               "AND timestamp IS NULL LIMIT 1");

        cmd.Parameters.AddWithValue("opponent1", NpgsqlDbType.Uuid, opponent1);
        cmd.Parameters.AddWithValue("opponent2", NpgsqlDbType.Uuid, opponent2);

        var battleId = cmd.ExecuteScalar() as Guid?;
        
        // if there is a battle with the given opponents, return true
        if (battleId != null)
        {
            _semaphore.Release();
            return new Tuple<Guid, bool>((Guid)battleId, true);
        }
    
        // create a new battle with the given opponents
        await using var cmd2 = db.CreateCommand("INSERT INTO battle (opponent_1, opponent_2) VALUES (@opponent1, @opponent2) RETURNING id");
        cmd2.Parameters.AddWithValue("opponent1", NpgsqlDbType.Uuid, opponent1);
        cmd2.Parameters.AddWithValue("opponent2", NpgsqlDbType.Uuid, opponent2);

        battleId = (Guid)((await cmd2.ExecuteScalarAsync()) ?? throw new Exception("Failed to create battle"));
        _semaphore.Release();
        return new Tuple<Guid, bool>((Guid)battleId, false);
    }

    public static async Task ConcludeBattle(Guid battle, JObject log)
    {
        await _semaphore.WaitAsync();

        // set timestamp to current time
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("UPDATE battle SET timestamp = CURRENT_TIMESTAMP, log = @log WHERE id = @battle_id");
        cmd.Parameters.AddWithValue("battle_id", NpgsqlDbType.Uuid, battle);
        cmd.Parameters.AddWithValue("log", NpgsqlDbType.Jsonb, log.ToString());
        
        await cmd.ExecuteNonQueryAsync();
        _semaphore.Release();
    }

    public static async Task<bool> IsUserParticipantInBattle(Guid battleId, Guid userId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("SELECT id FROM battle WHERE id = @battle_id AND (opponent_1 = @user_id OR opponent_2 = @user_id) LIMIT 1");
        cmd.Parameters.AddWithValue("battle_id", NpgsqlDbType.Uuid, battleId);
        cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, userId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        return reader.HasRows;
    }
    
    public static async Task<object?> GetBattle(Guid battleId)
    {
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("SELECT opponent_1, opponent_2, timestamp, log FROM battle WHERE id = @battle_id LIMIT 1");
        cmd.Parameters.AddWithValue("battle_id", NpgsqlDbType.Uuid, battleId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (!reader.HasRows)
            return null;
        
        await reader.ReadAsync();
        
        return new
        {
            players = new []{reader.GetGuid(0), reader.GetGuid(1)},
            timestamp = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
            log = reader.IsDBNull(3) ? null : JsonSerializer.Deserialize<object?>(reader.GetString(3))
        };
    }
    
    public static async Task UpdateStats(Guid winner, Guid loser)
    {
        await _semaphore.WaitAsync();
        await using var db = Connection.GetDataSource();
        
        await using var cmd = db.CreateCommand("UPDATE \"user\" SET wins = wins + 1 WHERE id = @winner_id");
        cmd.Parameters.AddWithValue("winner_id", NpgsqlDbType.Uuid, winner);
        
        await cmd.ExecuteNonQueryAsync();
        
        await using var cmd2 = db.CreateCommand("UPDATE \"user\" SET losses = losses + 1 WHERE id = @loser_id");
        cmd2.Parameters.AddWithValue("loser_id", NpgsqlDbType.Uuid, loser);
        
        // TODO: CALCULATE ELO
        
        await cmd2.ExecuteNonQueryAsync();
        _semaphore.Release();
    }
}