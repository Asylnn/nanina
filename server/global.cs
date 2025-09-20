using Newtonsoft.Json;
using WebSocketSharp.Server;
using Nanina.Communication;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Nanina.UserData.ModifierData;
using Nanina.Crafting;
using Nanina.Activities;
using Nanina.Gacha;
using Nanina.Dungeon;

namespace Nanina;

public static class Global
{
    public static readonly Dictionary<long, System.Timers.Timer> activityTimers = [];
    public static readonly Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../config.json"))!;
    public static readonly Dictionary<string, Banner> banners = JsonConvert.DeserializeObject<Dictionary<string, Banner>>(File.ReadAllText(config.banners_storage_path))!;
    public static readonly Dictionary<string, Waifu> waifus = JsonConvert.DeserializeObject<Dictionary<string, Waifu>>(File.ReadAllText("../save/waifu.json"))!;
    public static readonly Dictionary<short, Item> items = JsonConvert.DeserializeObject<Dictionary<short, Item>>(File.ReadAllText("../save/item.json"))!;
    public static readonly Dictionary<short, Equipment> equipments = JsonConvert.DeserializeObject<Dictionary<short, Equipment>>(File.ReadAllText("../save/equipment.json"))!;
    public static readonly Dictionary<short, Set> sets = JsonConvert.DeserializeObject<Dictionary<short, Set>>(File.ReadAllText("../save/set.json"))!;
    public static readonly BaseValues baseValues = JsonConvert.DeserializeObject<BaseValues>(File.ReadAllText("../baseValues.json"))!;
    public static readonly Maimai.Chart[] charts = JsonConvert.DeserializeObject<Maimai.Chart[]>(File.ReadAllText("../charts.json"))!;
    public static readonly List<List<Loot>> userLevelRewards = LoadUserLevelRewards();
    public static readonly List<EquipmentAttribute> baseAttributes = JsonConvert.DeserializeObject<List<EquipmentAttribute>>(File.ReadAllText("../save/attributes.json"))!;
    public static readonly Dictionary<string, ResearchNode> researchNodes = JsonConvert.DeserializeObject<Dictionary<string, ResearchNode>>(File.ReadAllText("../save/research.json"))!;
    public static readonly Dictionary<short, Craft> craftingRecipes = LoadCraftingRecipes();
    public static readonly Dictionary<string, Template> dungeons = JsonConvert.DeserializeObject<Dictionary<string, Template>>(File.ReadAllText(config.dungeon_storage_path))!;
    public static readonly List<ExplorationLoot> explorationLoot = JsonConvert.DeserializeObject<List<ExplorationLoot>>(File.ReadAllText("../save/exploration_loot.json"))!;
    public static readonly UpgradeRequirement upgradeRequirements = JsonConvert.DeserializeObject<UpgradeRequirement>(File.ReadAllText("../save/upgrade_requirements.json"))!;
    public static readonly string craftingRecipesString = JsonConvert.SerializeObject(craftingRecipes);
    public static readonly string dungeonsString = JsonConvert.SerializeObject(dungeons);
    public static readonly string researchNodesString = JsonConvert.SerializeObject(researchNodes);
    public static readonly string bannersString = JsonConvert.SerializeObject(banners);

    //If at release it's only used to send the data to the client, then just send it without deserializing0

    public static WebSocketServer ws;

    private static List<List<Loot>> LoadUserLevelRewards()
    {
        var userLevelRewardsJson = JsonConvert.DeserializeObject<List<List<LiteLoot>>>(File.ReadAllText("../save/user_level_rewards.json"))!;
        List<List<Loot>> userLevelRewards = [];
        for (var i = 0; i < userLevelRewardsJson.Count; i++)
        {
            userLevelRewards.Add(new List<Loot>());
            foreach (var reward in userLevelRewardsJson[i])
            {
                var loot = new Loot
                {
                    lootType = reward.lootType,
                    amount = reward.amount,
                    item = null
                };
                if(reward.itemId != -1)
                    loot.item = items[reward.itemId];
                userLevelRewards[i].Add(loot);
            }
        }
        return userLevelRewards;
    }

    private static Dictionary<short, Craft> LoadCraftingRecipes()
    {
        var temp_craftingRecipes = JsonConvert.DeserializeObject<List<Craft>>(File.ReadAllText("../save/crafts.json"))!;
        Dictionary<short, Craft> craftingRecipes = [];
        foreach(var craftRecipe in temp_craftingRecipes)
        {
            foreach(var craftIngredient in  (List<CraftIngredient>)[.. craftRecipe.ingredients, .. craftRecipe.results] )
            {
                craftIngredient.imgPATH = items[craftIngredient.id].imgPATH;
            }
            craftingRecipes[craftRecipe.id] = craftRecipe;
        }
        return craftingRecipes;
    }
}
