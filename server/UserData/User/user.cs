using System.Xml.XPath;
using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Nanina.UserData
{

    public class User(string username, Ids ids)
    {
        public bool admin {get; set;} = false;
        public byte lvl {get; set;} = 1;
        public uint xp {get; set;} = 0;
        public uint XpToLvlUp {
            get => 40u + lvl*10u;
        }
        public bool isInDungeon {get; set;}
        public ulong dungeonInstanceId {get; set;}
        public Game preferedGame {get; set;} = Game.OsuStandard;
        public long lvlRewards {get; set;}
        public string activeSessionId {get; set;} = null;
        public double max_energy {get; set;} = Global.baseValues.base_max_energy;
        public double energy {get; set;} = Global.baseValues.base_max_energy;
        public bool isRegenerating {get; set;} = false;
        public string username { get; set; } = username;
        public List<Waifu> waifus { get; set; } = [Utils.DeepCopyReflection(Global.waifus.Find(x => x.id == "0"))];
        public string Id { get; set; } = CreateId();
        public string theme { get; set; } = Global.baseValues.base_theme;
        public Ids ids { get; set; } = ids;
        public Tokens tokens { get; set; } = new ();
        public string locale { get; set; } = Global.config.default_locale;  
        public string avatarPATH { get; set; } = ""; //Unused
        public StatCount statCount { get; set; } = new();
        public Dictionary<Game, string[]> fightHistory { get; set; } = new();
        public Fight fight { get; set; } = new Fight();
        public uint gacha_currency { get; set; } = Global.baseValues.base_gacha_currency_amount;
        public Dictionary<string, PullBannerHistory> pullBannerHistory { get; set; } = [];
        public Verification verification { get; set; } = new() { osuVerificationCode=null, osuVerificationCodeTimestamp = 0, isOsuIdVerified=false };
        public ulong claimTimestamp { get; set; } = 0;
        public Inventory inventory { get; set; } = new ();

        private static string CreateId()
        {   
            Random rng = new Random();
            return Utils.GetTimestamp().ToString() + (rng.Next(89_999_999) + 10_000_000).ToString();
        }

        public (double energy, uint gc) SpendEnergy(double ratio)
        {
            var spent_energy = energy*Global.baseValues.proportion_of_energy_used_for_each_action*ratio;
            energy -= spent_energy;
            var gc = (uint) Math.Ceiling(spent_energy*Global.baseValues.spent_energy_to_gacha_currency_conversion_rate);
            gacha_currency += (uint) Math.Floor(gc*ratio);
            return (energy:spent_energy + Global.baseValues.free_energy_not_used_for_each_action*ratio, gc);
        }

        public void GetXP(uint _xp)
        {
            xp += _xp;
            if(xp >= XpToLvlUp){
                xp -= XpToLvlUp;
                uint temp_xp = xp;
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

            PeriodicTimer energyRegenTimer = new (new (Global.baseValues.energy_regen_tick_in_seconds*10_000_000)); //units are 100ns
            while (await energyRegenTimer.WaitForNextTickAsync())
            {
                var u = DBUtils.Get<UserData.User>(x => x.Id == id);
                
                u.energy += Global.baseValues.energy_regen_tick_amount_in_percent/100d*u.max_energy;
                if(u.energy >= u.max_energy)
                {
                    u.isRegenerating = false;
                    u.energy = u.max_energy;
                }
                    

                DBUtils.Update(u);


                var session = DBUtils.Get<Session>(x => x.id == u.activeSessionId);
                if(session is not null){
                    if(session.webSocketId is not null)
                    {
                        Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                        {
                            type = "user",
                            data = JsonConvert.SerializeObject(u),
                        }), session.webSocketId);
                    }
                }
                
                
                
                
                if(u.energy >= u.max_energy){
                    energyRegenTimer.Dispose();
                }
            }
        }

        public bool CheckRewardAvailability(byte level)
        {
            return (Convert.ToInt64(Math.Pow(2, level))&lvlRewards) == 0;
        }

        public void UseItem(Item item)
        {
            foreach(var modifier in item.modifiers)
            {
                switch(modifier.stat)
                {
                    case StatModifier.MaxEnergy:
                        this.max_energy += modifier.amount;
                        this.energy += modifier.amount;
                        break;
                }
            }
        }
    }
}
