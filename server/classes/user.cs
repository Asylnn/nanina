using Newtonsoft.Json;
using System.Linq;

static class UserId {
    static public string CreateId()
    {   
        Random rng = new Random();
        return Utils.GetTimestamp().ToString() + rng.Next(10000000).ToString();
    }
}

public class StatCount {
    public int std_claim_count { get; set; } 

}

public class PocoUser
{
    public bool admin {get; set;}
    public string username { get; set; } 
    public PocoWaifu waifu { get; set; } //OLD!! To delete soon when all users migrated
    public List<PocoWaifu> waifus { get; set; }
    public string Id { get; set; }
    public string theme { get; set; }
    public Ids ids { get; set; }
    public Tokens tokens { get; set; }
    public string locale { get; set; }
    public string avatarPATH { get; set; }
    public StatCount statCount { get; set; }
    public List<Fight> fights { get; set; }
    public int gacha_currency { get; set; }
    public Dictionary<string, PullBannerHistory> pullBannerHistory { get; set; }
    public Verification verification { get; set; }
    public long claimTimestamp { get; set; }

} 

public class Ids {
    public string discordId { get; set; } //Does this work?

    public string osuId { get; set; }
}

public class Tokens {
    public string discord_access_token { get; set; }
    public string discord_refresh_token { get; set; }
}

public class Verification {
    public long osuVerificationCodetimestamp { get; set; }
    public bool isOsuIdVerified { get; set; }
    public string osuVerificationCode { get; set; }
}

public class Fight {
    public string game { get; set; }
    public string id { get; set; }
    public long timestamp { get; set; }
    public bool completed { get; set; }
}

public class PullBannerHistory {
    public List<string> pullHistory { get; set; }
    public short pullBeforePity { get; set; }
}

public class User {
    public string locale = "us-en";
    public string username;
    public bool admin = false;
    public List<Waifu> waifus = [Waifu.FromPoco(DBUtils.GetWaifu("0"))];
    public string Id = UserId.CreateId();
    public Ids ids = new()  {discordId="1", osuId="1"};
    public List<Fight> fights;
    public Tokens tokens;
    public string avatarPATH;
    public StatCount statCount = new();
    private string theme = "dark_theme";
    public int gacha_currency;
    public Dictionary<string, PullBannerHistory> pullBannerHistory;
    public Verification verification = new() { osuVerificationCode=null, osuVerificationCodetimestamp = 0, isOsuIdVerified=false };
    public long claimTimestamp;
    public User(string username, Ids ids)
    
    
    {
        //Id = UserId.CreateId();
        this.username = username;
        this.ids = ids;
    }
    public PocoUser ToPoco()
    {
        return new PocoUser
        {
            admin = admin,
            username = username,
            tokens = tokens,
            waifus = waifus.Select(waifu => waifu.ToPoco()).ToList(),
            Id = Id,
            theme = theme,
            ids = ids,
            avatarPATH = avatarPATH,
            locale = locale,
            statCount = statCount,
            fights = fights,
            gacha_currency = gacha_currency,
            pullBannerHistory = pullBannerHistory,
            verification = verification,
            claimTimestamp = claimTimestamp,
        };
    }
    public PocoUser ToPocoServer(){
        var poco = ToPoco();
        poco.tokens.discord_access_token = "";
        poco.tokens.discord_refresh_token = "";
        return poco;
    }
    public static User FromPoco(PocoUser poco)
    {
        User user = new (poco.username, poco.ids)
        {
            Id = poco.Id,
            waifus = poco.waifus.Select(Waifu.FromPoco).ToList(),
            theme = poco.theme,
            ids = poco.ids,
            avatarPATH = poco.avatarPATH,
            locale = poco.locale,
            statCount = poco.statCount,
            admin = poco.admin,
            fights = poco.fights,
            tokens = poco.tokens,
            gacha_currency = poco.gacha_currency,
            pullBannerHistory = poco.pullBannerHistory,
            verification = poco.verification,
            claimTimestamp = poco.claimTimestamp
        };
        return user;
    }

}