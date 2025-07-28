using Nanina.UserData.WaifuData;
using Nanina.Communication;
using System.ComponentModel;

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
        public List<Loot> loot { get; set; } = [];

        public void Timeout(Waifu waifu)
        {
            switch(type)
            {
                case ActivityType.Cafe:
                    CafeTimeout(waifu);
                    break;
                case ActivityType.Mining:
                    MiningTimeout(waifu);
                    break;
            }
        }
        public void CafeTimeout(Waifu waifu)
        {
            var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
            var money = (uint) Math.Ceiling((waifu.Kaw + waifu.Luck)*statRandomness);
            var loot = new Loot()
            {
                lootType = LootType.Money,
                amount = money,
            };
            finished = true;
            this.loot = [loot];
        }
        public void MiningTimeout(Waifu waifu)
        {
            finished = true;

            foreach(int tier in Enumerable.Range(1,4))
            {
                var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
                var miningPower = (uint) Math.Ceiling((waifu.Str + waifu.Luck)*statRandomness);
                
                double qty = tier switch
                {
                    1 => 1d + Math.Sqrt(miningPower / 10d),
                    2 => miningPower > 150 ? Math.Sqrt((miningPower - 100d) / 10d) - 2d : 0d,
                    3 => miningPower > 300 ? Math.Sqrt((miningPower - 230d) / 15d) - 2d : 0d,
                    4 => miningPower > 600 ? Math.Sqrt((miningPower - 500d) / 20d) - 2d : 0d,
                    _ => 0d,
                };
                var wholeQty = Math.Truncate(qty);
                var fracQty = qty - wholeQty;
                if(new Random().NextDouble() <= fracQty)
                    wholeQty++;

                if(wholeQty >= 1)
                {
                    loot.Add(new ()
                    {
                        lootType = LootType.Item,
                        amount = (ushort)wholeQty,
                        item = Utils.DeepCopyReflection(Global.items.Find(item => item.id == 5 + tier)),
                    });
                }
            }
        }
    }
}