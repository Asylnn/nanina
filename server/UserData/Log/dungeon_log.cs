namespace Nanina.UserData.Log;

public class DungeonLog(short[] waifuLVL, short floor, long clearTime, float[] claimDmg)
{
    public short floor = floor;
    public short[] waifuLVL { get; set; } = waifuLVL;
    public long timestamp { get; set; } = Utils.GetTimestamp();
    public long clearTime { get; set; } = clearTime;
    public float[] claimDmg { get; set; } = claimDmg;
}