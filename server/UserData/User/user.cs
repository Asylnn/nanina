using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.UserData
{

    public class User(string username, Ids ids)
    {
        public bool admin {get; set;} = false;
        public string activeSessionId {get; set;} = null;
        public double max_energy {get; set;} = Global.baseValues.base_max_energy;
        public double energy {get; set;} = Global.baseValues.base_max_energy;
        public bool isRegenerating {get; set;} = false;
        public string username { get; set; } = username;
        public List<Waifu> waifus { get; set; } = [DBUtils.Get<Waifu>(x => x.id == "0")];
        public string Id { get; set; } = CreateId();
        public string theme { get; set; } = Global.baseValues.base_theme;
        public Ids ids { get; set; } = ids;
        public Tokens tokens { get; set; } = new ();
        public string locale { get; set; } = Global.config.default_locale;  
        public string avatarPATH { get; set; } = ""; //Unused
        public StatCount statCount { get; set; } = new();
        public Dictionary<string, string[]> fightHistory = [];
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

        public (double energy, uint gc) SpendEnergy()
        {
            var spent_energy = energy*Global.baseValues.proportion_of_energy_used_for_each_action;
            energy -= spent_energy;
            var gc = (uint) Math.Ceiling(spent_energy*Global.baseValues.spent_energy_to_gacha_currency_conversion_rate);
            gacha_currency += gc;
            return (energy:spent_energy + Global.baseValues.free_energy_not_used_for_each_action, gc);
        }

        public static async void RegenEnergy(User user)
        {
            Console.WriteLine("Regen Energy");
            if(user.isRegenerating) return;

            
            Console.WriteLine("Effectively Regen Energy");

            var id = user.Id;
            user.isRegenerating = true;
            DBUtils.Update(user);

            PeriodicTimer energyRegenTimer = new (new (Global.baseValues.energy_regen_tick_in_seconds*10_000_000)); //units are 100ns
            while (await energyRegenTimer.WaitForNextTickAsync())
            {
                var u = DBUtils.Get<UserData.User>(x => x.Id == id);
                Console.WriteLine("Regen Tick");
                
                u.energy += Global.baseValues.energy_regen_tick_amount;
                if(u.energy >= u.max_energy)
                {
                    Console.WriteLine("Max Energy reached");
                    u.isRegenerating = false;
                    u.energy = u.max_energy;
                }
                    

                DBUtils.Update(u);


                var session = DBUtils.Get<Session>(x => x.id == u.activeSessionId);
                Console.WriteLine(JsonConvert.SerializeObject(u.activeSessionId));

                Console.WriteLine(JsonConvert.SerializeObject(session));
                if(session is not null){
                    Console.WriteLine("step 1 reached");
                    if(session.webSocketId is not null)
                    {
                        Console.WriteLine("Updating User");
                        Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                        {
                            type = "user",
                            data = JsonConvert.SerializeObject(u),
                        }), session.webSocketId);
                    }
                }
                
                
                
                
                if(u.energy >= u.max_energy){
                    Console.WriteLine("Disposing Timer");
                    
                    energyRegenTimer.Dispose();
                }
            }
        }
    }
}
