using Nanina.Database;
using Nanina.UserData.WaifuData;

namespace Nanina.UserData
{

    public class User(string username, Ids ids) 
    {
        public bool admin {get; set;} = false;
        public string username { get; set; } = username;
        public Waifu waifu { get; set; } //OLD!! To delete soon when all users migrated
        public List<Waifu> waifus { get; set; } = [DBUtils.GetWaifu("0")];
        public string Id { get; set; } = CreateId();
        public string theme { get; set; } = "dark_theme";
        public Ids ids { get; set; } = ids;
        public Tokens tokens { get; set; } = new ();
        public string locale { get; set; } = "en";
        public string avatarPATH { get; set; } = "";
        public StatCount statCount { get; set; } = new();
        public List<Fight> fights { get; set; } = [];
        public uint gacha_currency { get; set; } = 0;
        public Dictionary<string, PullBannerHistory> pullBannerHistory { get; set; }
        public Verification verification { get; set; } = new() { osuVerificationCode=null, osuVerificationCodetimestamp = 0, isOsuIdVerified=false };
        public ulong claimTimestamp { get; set; }
        public Inventory inventory { get; set; }
        private static string CreateId()
        {   
            Random rng = new Random();
            return Utils.GetTimestamp().ToString() + rng.Next(99_999_999).ToString();
        }

        /*public PocoUser ToPocoServer(){
            var poco = ToPoco();
            poco.tokens.discord_access_token = "";
            poco.tokens.discord_refresh_token = "";
            return poco;
        }*/
    }
}
