namespace Nanina.UserData.Log;

public class PassiveClaimLog(Game game, float timesave, float difficultyRating)
{
    public Game game { get; set; } = game;
    public float timesave { get; set; } = timesave;
    public long timestamp { get; set; } = Utils.GetTimestamp();
    public float difficultyRating { get; set; } = difficultyRating;
}