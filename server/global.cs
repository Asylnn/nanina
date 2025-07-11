using LiteDB;
using Newtonsoft.Json;
using WebSocketSharp.Server;
using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;

namespace Nanina
{
    public static class Global {
        public static readonly Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../config.json"));
        public static readonly List<Waifu> waifus = JsonConvert.DeserializeObject<List<Waifu>>(File.ReadAllText("../save/waifu.json"));
        public static readonly List<Item> items = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText("../save/item.json"));
        public static readonly List<Equipment> equipments = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText("../save/equipment.json"));
        public static readonly List<Set> sets = JsonConvert.DeserializeObject<List<Set>>(File.ReadAllText("../save/set.json"));
            
        public static readonly BaseValues baseValues = JsonConvert.DeserializeObject<BaseValues>(File.ReadAllText("../baseValues.json"));
        public static readonly Maimai.Chart[] charts = JsonConvert.DeserializeObject<Maimai.Chart[]>(File.ReadAllText("../charts.json"));
        public static readonly List<List<Loot>> userLevelRewards = LoadUserLevelRewards();
        public static WebSocketServer ws;

        private static List<List<Loot>> LoadUserLevelRewards()
        {
            var userLevelRewardsJson = JsonConvert.DeserializeObject<List<List<LiteLoot>>>(File.ReadAllText("../save/user_level_rewards.json"));
            List<List<Loot>> userLevelRewards = [];
            for(var i = 0; i < userLevelRewardsJson.Count ; i++)
            {
                userLevelRewards.Add(new List<Loot>());
                foreach(var reward in userLevelRewardsJson[i])
                {
                    var loot = new Loot
                    {
                        lootType = reward.lootType,
                        amount = reward.amount,
                        item = null
                    };
                    if(reward.itemId is not null)
                    {
                        loot.item = items.Find(x => x.id == reward.itemId);
                        loot.item.count = loot.amount;
                    }
                        
                    
                    

                    userLevelRewards[i].Add(loot);
                }
            }
            return userLevelRewards;
        }
    }

    
}
