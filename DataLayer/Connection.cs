using Npgsql;

namespace DataLayer;


public static class Connection
{
    private const string ConnectionString = "Host=localhost:5432;Username=swe1user;Password=swe1pw;Database=mctg";

    public static NpgsqlDataSource GetDataSource()
    {
        return NpgsqlDataSource.Create(ConnectionString);
    } 
    
}