namespace GameLogic;

public class Elo
{
    private const int K = 15;
    
    public const double Win = 1;
    public const double Draw = 0.5;
    public const double Loss = 0;

    public int Rating { get; private set; }

    public Elo (int rating)
    {
        Rating = rating;
    }

    public double GetExpectedScore(Elo opponent)
    {
        return 1 / (1 + Math.Pow(10, ((double)opponent.Rating - Rating) / 400));
    } 
    
    public void UpdateScore(Elo opponent, double score)
    {
        Rating += (int)Math.Floor(K * (score - GetExpectedScore(opponent)));
    }
}