using Nanina.UserData.WaifuData;
using Nanina.Communication;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Maimai;

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
        public long id { get; set; } = Utils.CreateIdLong();
        public long timestamp { get; set; } = Utils.GetTimestamp();
        public bool finished { get; set; } = false;
        public required ActivityType type { get; set; }
        public long Timeout 
        {
            set 
            {
                timeout = value;
                originalTimeout = value;
            } 
        }
        public long originalTimeout { get; set; }
        public long timeout { get; set; }
        public required string waifuID { get; set; }
        public List<Loot> loot { get; set; } = [];
        public string? researchID { get; set; }


        public void OnTimeout(User user)
        {
            finished = true;
            double stat = 0;

            switch (type)
            {
                case ActivityType.Cafe:
                    OnCafeTimeout(user.waifus[waifuID]);
                    stat = user.waifus[waifuID].Kaw;
                    break;
                case ActivityType.Mining:
                    OnMiningTimeout(user.waifus[waifuID]);
                    stat = user.waifus[waifuID].Str;
                    break;
                case ActivityType.Research:
                    OnResearchTimeout(user);
                    stat = user.waifus[waifuID].Int;
                    break;
                case ActivityType.Crafting:
                    stat = user.waifus[waifuID].Dex;
                    //since loot is already created at the moment the activity is created (since it's more practical this way), there is no need to do stuff here 
                    //maybe do the same thing with research? Yeah difinitively
                    break;
                case ActivityType.Exploration:
                    OnExplorationTimeout(user.waifus[waifuID]);
                    stat = user.waifus[waifuID].Agi;
                    break;
            }
            user.activityLog.Add(new(type, (float)stat));
        }
        private void OnCafeTimeout(Waifu waifu)
        {
            var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
            var money = (int) Math.Ceiling((waifu.Kaw + waifu.Luck)*statRandomness*9*2);
            var loot = new Loot()
            {
                lootType = LootType.Money,
                amount = money,
            };
            this.loot = [loot];
        }
        private void OnMiningTimeout(Waifu waifu)
        {

            foreach(int tier in Enumerable.Range(1,4))
            {
                var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
                var miningPower = (int) Math.Ceiling((waifu.Str + waifu.Luck)*statRandomness);
                
                double qty = tier switch
                {
                    1 => 1d + Math.Sqrt(miningPower),
                    2 => miningPower > 150 ? Math.Sqrt(miningPower / 1.2d) - 10d: 0d,
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
                        amount = (short)wholeQty,
                        item = Utils.DeepCopyReflection(Global.items[(short)(5 + tier)]),
                    });
                }
            }
        }

        private void OnResearchTimeout(User user)
        {
            var researchNode = Global.researchNodes[researchID!];
            var vehicleItem = Utils.DeepCopyReflection(Global.items[0])!;

            
            if(user.completedResearches.ContainsKey(researchID!))
                user.completedResearches[researchID!] += 1;
            else
                user.completedResearches[researchID!] = 1;

            /*Right now we only give the unlocks/else after the user clicked the "finish research" button, and those unlocks go through
            a vehicleItem's modifiers. We could do it right now though, but i don't know which is better*/

            vehicleItem.modifiers = researchNode.modifiers;
            loot.Add(new ()
            {
                lootType = LootType.Modifiers,
                item = vehicleItem,
            });
        }
        
        private void OnExplorationTimeout(Waifu waifu)
        {
            var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
            var explorationPower = Math.Ceiling((waifu.Agi + waifu.Luck)*statRandomness);
            var lootPool = Global.explorationLoot.FindAll(loot => loot.powerRequired <= explorationPower);
            var totalWeight = lootPool.Aggregate(0d, (weight, loot) => weight + loot.weight);
            while(explorationPower > 0)
            {
                var currentWeight = new Random().NextDouble()*totalWeight;
                var explorationLoot = lootPool.First(loot => //That actually good, a bit proud of myself
                {
                    currentWeight -= loot.weight;
                    return currentWeight <= 0;
                });
                explorationPower -= explorationLoot.cost;
                loot.Add(new()
                {
                    lootType = LootType.Item,
                    item = Global.items[explorationLoot.itemId],
                    amount = 1,
                });
            }
        }

        public static long GetResearchTimeout(Waifu waifu, double cost)
        {
            return (long) (cost / (waifu.Int + waifu.Luck) * 3600d  /*1000d*/);
        }

        public static long GetCraftingTimeout(Waifu waifu, double cost)
        {
            return (long) (cost / (waifu.Dex + waifu.Luck) * 3600d / 2 * 1000d);
        }
        
    }
}