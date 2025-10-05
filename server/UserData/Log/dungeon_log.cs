namespace Nanina.UserData.Log;

public class DungeonLog(short[] waifuLVL, short floor, long clearTime, double[] claimDmg)
{
    public short floor = floor;
    public short[] waifuLVL { get; set; } = waifuLVL;
    public long timestamp { get; set; } = Utils.GetTimestamp();
    public long clearTime { get; set; } = clearTime;
    public double[] claimDmg { get; set; } = claimDmg;
}