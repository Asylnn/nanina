namespace Nanina.UserData.Log;

public class ClaimLog(Game game, float xp, float difficultyRating)
{
    public Game game { get; set; } = game;
    public float xp { get; set; } = xp;
    public long timestamp { get; set; } = Utils.GetTimestamp();
    public float difficultyRating { get; set; } = difficultyRating;
}