namespace Models;

public class Stats
{
    public string Name { get; set; }
    public int Elo { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    
    public Stats(string name, int elo, int wins, int losses)
    {
        Name = name;
        Elo = elo;
        Wins = wins;
        Losses = losses;
    }
}