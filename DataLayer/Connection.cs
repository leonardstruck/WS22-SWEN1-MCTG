using Npgsql;

namespace DataLayer;


public sealed class Connection
{
    private const string ConnectionString = "Host=localhost:5432;Username=swe1user;Password=swe1pw;Database=mctg";

    private Connection()
    {
        DataSource = NpgsqlDataSource.Create(ConnectionString);
    }

    private static Connection? _instance;
    public NpgsqlDataSource DataSource { get; private set; }

    public static Connection GetInstance()
    {
        return _instance ??= new Connection();
    }
    
}