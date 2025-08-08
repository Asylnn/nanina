using Nanina.UserData.WaifuData;
using Nanina.Communication;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        public required ActivityType type { get; set; }
        public ulong Timeout 
        {
            set 
            {
                timeout = value;
                originalTimeout = value;
            } 
        }
        public ulong originalTimeout { get; set; }
        public ulong timeout { get; set; }
        public required string waifuID { get; set; }
        public List<Loot> loot { get; set; } = [];
        public string? researchID { get; set; }
        

        public void OnTimeout(User user)
        {
            finished = true;
            switch(type)
            {
                case ActivityType.Cafe:
                    OnCafeTimeout(user.waifus.Find(waifu => waifu.id == waifuID)!);
                    break;
                case ActivityType.Mining:
                    OnMiningTimeout(user.waifus.Find(waifu => waifu.id == waifuID)!);
                    break;
                case ActivityType.Research:
                    OnResearchTimeout(user);
                    break;
                case ActivityType.Crafting:
                    //since loot is already created at the moment the activity is created (since it's more practical this way), there is no need to do stuff here 
                    //maybe do the same thing with research? Yeah difinitively
                    break;
                case ActivityType.Exploration:
                    OnExplorationTimeout(user.waifus.Find(waifu => waifu.id == waifuID)!);
                    break;
            }
        }
        private void OnCafeTimeout(Waifu waifu)
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
        private void OnMiningTimeout(Waifu waifu)
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
                        item = Utils.DeepCopyReflection(Global.items.Find(item => item.id == 5 + tier))!,
                    });
                }
            }
        }

        private void OnResearchTimeout(User user)
        {
            var researchNode = Global.researchNodes.Find(RN => RN.id == researchID)!;
            var vehicleItem = Utils.DeepCopyReflection(Global.items.Find(i => i.id == 0))!;

            var completedResearch = user.completedResearches.Find(r => r.id == researchID);
            if(completedResearch == null)
                user.completedResearches.Add(new CompletedResearch
                {
                    id=researchID!,
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
        
        private void OnExplorationTimeout(Waifu waifu)
        {
            var statRandomness = Utils.RandomMultiplicator(Global.baseValues.cafe_reward_randomness);
            var explorationPower = Math.Ceiling((waifu.Agi + waifu.Luck)*statRandomness);
            Console.WriteLine(explorationPower);
            var lootPool = Global.explorationLoot.Where(loot => loot.powerRequired <= explorationPower);
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
                    item = Global.items.Find(item => item.id == explorationLoot.itemId)!,
                    amount = 1,
                });
            }
        }

        public static ulong GetResearchTimeout(Waifu waifu, double cost)
        {
            return (ulong) (cost / (waifu.Int + waifu.Luck) * 3600d * 1000d);
        }

        public static ulong GetCraftingTimeout(Waifu waifu, double cost)
        {
            return (ulong) (cost / (waifu.Dex + waifu.Luck) * 1800d * 1000d);
        }
        
    }
}