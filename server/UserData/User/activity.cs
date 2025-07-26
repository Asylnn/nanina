using Nanina.UserData.WaifuData;
using Nanina.Communication;

namespace Nanina.UserData
{
    public enum ActivityType
    {
        Cafe,
        Exploration,
        Crafting,
        Research,
        Mining,
    }

    public class Activity
    {
        public ulong id { get; set; } = Utils.CreateIdUlong();
        public ulong timestamp { get; set; } = Utils.GetTimestamp();
        public bool finished { get; set; } = false;
        public ActivityType type { get; set; }
        public ulong timeout { get; set; }
        public string waifuID { get; set; }
        public List<Loot> loot { get; set; }
        public static double ConcludeCafeActivity(Waifu waifu)
        {
            var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
            return (waifu.Kaw + waifu.Luck)*statRandomness;
        }
    }
}