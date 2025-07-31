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
        public string researchID { get; set; }

        public void Timeout(User user)
        {
            finished = true;
            switch(type)
            {
                case ActivityType.Cafe:
                    CafeTimeout(user.waifus.Find(waifu => waifu.id == waifuID));
                    break;
                case ActivityType.Mining:
                    MiningTimeout(user.waifus.Find(waifu => waifu.id == waifuID));
                    break;
                case ActivityType.Research:
                    OnResearchTimeout(user);
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
            this.loot = [loot];
        }
        public void MiningTimeout(Waifu waifu)
        {

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

        public void OnResearchTimeout(User user)
        {
            var researchNode = Global.researchNodes.Find(RN => RN.id == researchID);
            var vehicleItem = Utils.DeepCopyReflection(Global.items.Find(i => i.id == 0));

            var completedResearch = user.completedResearches.Find(r => r.id == researchID);
            if(completedResearch == null)
                user.completedResearches.Add(new CompletedResearch
                {
                    id=researchID,
                    amount=1,
                });
            else
                completedResearch.amount++;

            /*Right now we only give the unlocks/else after the user clicked the "finish research" button, and those unlocks go through
            a vehicleItem's modifiers. We could do it right now though, but i don't know which is better*/

            vehicleItem.modifiers = researchNode.modifiers;
            loot.Add(new ()
            {
                lootType = LootType.Modifiers,
                item = vehicleItem,
            });
        }

        public static ulong GetResearchTimeout(Waifu waifu, double cost)
        {
            return (ulong) (cost / (waifu.Int + waifu.Luck) * 3600d * 1000d);
        }

        
    }
}