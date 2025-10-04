namespace Nanina.UserData.Log;
public class ActivityLog(ActivityType type, float stat)
{
    public ActivityType type { get; set; } = type;
    public float stat { get; set; } = stat;
    public long timestamp { get; set; } = Utils.GetTimestamp();
}