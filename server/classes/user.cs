public class StatCount {
    public int std_claim_count { get; set; } 

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
    public string locale { get; set; } = "us-en";
    public string avatarPATH { get; set; } = "";
    public StatCount statCount { get; set; } = new();
    public List<Fight> fights { get; set; } = [];
    public int gacha_currency { get; set; } = 0;
    public Dictionary<string, PullBannerHistory> pullBannerHistory { get; set; }
    public Verification verification { get; set; } = new() { osuVerificationCode=null, osuVerificationCodetimestamp = 0, isOsuIdVerified=false };
    public long claimTimestamp { get; set; }
    public Inventory inventory { get; set; }
    private static string CreateId()
    {   
        Random rng = new Random();
        return Utils.GetTimestamp().ToString() + rng.Next(10000000).ToString();
    }

    /*public PocoUser ToPocoServer(){
        var poco = ToPoco();
        poco.tokens.discord_access_token = "";
        poco.tokens.discord_refresh_token = "";
        return poco;
    }*/
}