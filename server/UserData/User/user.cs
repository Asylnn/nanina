using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.XPath;
using LiteDB;
using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Nanina.UserData
{

    public class User
    {
        public required string username { get; set; }
        public required Ids ids { get; set; }
        public int money { get; set; } = 0;
        public bool admin {get; set;} = false;
        public short lvl {get; set;} = 1;
        public int xp {get; set;} = 0;
        public int XpToLvlUp {
            get => 40 + lvl*10;
        }
        public List<Activity> activities { get; set; } = [];
        public int maxConcurrentActivities { get; set; } = 3;
        public bool isInDungeon { get; set; } = false;
        public List<string> waifuIdsInDungeon {get; set;} = [];
        //This is necessary since dungeons are stored only in memory and if the servers stop, we need to get back the waifus in the dungeon to put isDoingAction at false.
        public long dungeonInstanceId { get; set; } = 0;
        public Game preferedGame {get; set;} = Game.OsuStandard;
        public long lvlRewards { get; set; } = 0;
        public string? activeSessionId {get; set;} = null;
        public double max_energy {get; set;} = Global.baseValues.base_max_energy;
        public double energy {get; set;} = Global.baseValues.base_max_energy;
        public bool isRegenerating {get; set;} = false;
        public Dictionary<string, Waifu> waifus { get; set; } = new() { { "0", Utils.DeepCopyReflection(Global.waifus["0"])!} };
        public string Id { get; set; } = Utils.CreateId();
        public string theme { get; set; } = Global.baseValues.base_theme;
        public Tokens tokens { get; set; } = new ();
        public string locale { get; set; } = Global.config.default_locale;  
        public string avatarPATH { get; set; } = ""; //Unused
        public StatCount statCount { get; set; } = new();
        public Dictionary<Game, List<string>> fightHistory { get; set; } = [];
        public Fight? fight { get; set; }
        public int gacha_currency { get; set; } = Global.baseValues.base_gacha_currency_amount;
        public Dictionary<string, PullBannerHistory> pullBannerHistory { get; set; } = [];
        public Verification verification { get; set; } = new ();
        public long claimTimestamp { get; set; } = 0;
        public Inventory inventory { get; set; } = new ();
        public Dictionary<string, short> completedResearches { get; set; } = [];
        public Unlocks unlocks { get; set; } = new ();
        public long lastContinuousFightTimestamp { get; set; } = 0;
        public List<ContinuousFightLog> continuousFightLog { get; set; } = []; //Should maybe be dictionary?

        public (double energy, int gc) SpendEnergy(double ratio)
        {
            var spent_energy = energy * Global.baseValues.proportion_of_energy_used_for_each_action * ratio;
            energy -= spent_energy;
            var gc = (int)Math.Ceiling(spent_energy * Global.baseValues.spent_energy_to_gacha_currency_conversion_rate);
            gacha_currency += (int)Math.Floor(gc * ratio);
            RegenEnergy(this);
            return (energy: spent_energy + Global.baseValues.free_energy_not_used_for_each_action * ratio, gc);
            
        }

        public void GetXP(int _xp)
        {
            xp += _xp;
            if(xp >= XpToLvlUp){
                xp -= XpToLvlUp;
                int temp_xp = xp;
                lvl++; 
                xp = 0;
                GetXP(temp_xp);
            }
        }

        public static async void RegenEnergy(User user)
        {
            if(user.isRegenerating) return;
            var id = user.Id;
            user.isRegenerating = true;
            DBUtils.Update(user);
            UpdateEnergyOfClient(user);

            PeriodicTimer energyRegenTimer = new (new (Global.baseValues.energy_regen_tick_in_seconds*10_000_000)); //units are 100ns
            while (await energyRegenTimer.WaitForNextTickAsync())
            {
                user = DBUtils.Get<UserData.User>(x => x.Id == id)!;
                
                user.energy += Global.baseValues.energy_regen_tick_amount_in_percent/100d*user.max_energy;
                Console.WriteLine("u.max_energy");

                Console.WriteLine(user.energy);
                Console.WriteLine(user.max_energy);
                if (user.energy >= user.max_energy)
                {
                    user.isRegenerating = false;
                    user.energy = user.max_energy;
                    energyRegenTimer.Dispose();
                }
                    

                DBUtils.Update(user);
                UpdateEnergyOfClient(user);

            }
        }

        public static void UpdateEnergyOfClient(User user)
        {
            Console.WriteLine("update energy");
            Console.WriteLine(user.energy);
            var session = DBUtils.Get<Session>(x => x.id == user.activeSessionId);
            if(session is not null){
                if(session.webSocketId is not null)
                {
                    Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = ServerResponseType.UpdateEnergy,
                        data = user.energy.ToString(),
                    }), session.webSocketId);
                }
            }
        }

        public bool CheckRewardAvailability(byte level)
        {
            return (Convert.ToInt64(Math.Pow(2, level)) & lvlRewards) == 0;
        }

        public void UseUserConsumable(Item item)
        {
            foreach(var modifier in item.modifiers)
            {
                ApplyUserModifier(modifier);
            }
        }

        public void ApplyUserModifier(Modifier modifier)
        {
            switch(modifier.stat)
            {
                case StatModifier.MaxEnergy:
                    max_energy += modifier.amount;
                    energy += modifier.amount;
                    break;
                case StatModifier.UnlockFloor:
                    unlocks.maxDungeonFloor = (byte) modifier.amount;
                    break;
            }
        }

        public void ActivityTimeout(long activityId)
        {
            foreach (var activity in activities)
            {
                if (activity.id == activityId)
                {
                    //if cafe...
                    var session = DBUtils.Get<Session>(session => session.id == activeSessionId)!;

                    activity.OnTimeout(this);

                    if (session.webSocketId is not null)
                    {
                        Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                        {
                            type = ServerResponseType.ProvideCompletedActivity,
                            data = JsonConvert.SerializeObject(activity)
                        }), session.webSocketId);
                    }
                    DBUtils.Update(this);
                    return;
                }
            }
            Console.Error.WriteLine($"ActivityTimer finished with no associated activity, userId:{Id}, activityId:{activityId}");
        }
        /*
        public User() {}

        public User(BsonDocument bson){}
        */
    }
}
