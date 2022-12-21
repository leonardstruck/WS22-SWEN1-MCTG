namespace Models;

public class User
{
    public User(string username)
    {
        Username = username;
    }

    public string? Id { get; set; }
    public string Username { get; set; }

    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Image { get; set; }

    public int Coins { get; set; } = 20;
}