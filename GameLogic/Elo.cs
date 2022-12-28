namespace GameLogic;

public class Elo
{
    private const int K = 15;
    
    public const double WIN = 1;
    public const double DRAW = 0.5;
    public const double LOSS = 0;

    public int Rating { get; private set; }

    public Elo (int rating)
    {
        Rating = rating;
    }

    public double GetExpectedScore(Elo opponent)
    {
        return 1 / (1 + Math.Pow(10, ((double)opponent.Rating - (double)Rating) / 400));
    } 
    
    public void UpdateScore(Elo opponent, double score)
    {
        Rating += (int)Math.Floor(K * (score - GetExpectedScore(opponent)));
    }
}