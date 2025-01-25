using Newtonsoft.Json;

public class ServerTestToCreate 
{
    public string type { get; set; }
    public string message { get; set; }
    public short severity { get; set; }
}
static class UserId {
    static public string CreateId()
    {   
        Random rng = new Random();
        return Utils.GetTimestamp() + (string) rng.Next(10000000).ToString();
    }
}

public class StatCount {
    public int std_claim_count { get; set; } 

}

public class PocoUser
{
    public bool admin {get; set;}
    public string username { get; set; } 
    public PocoWaifu waifu { get; set; }
    public string Id { get; set; }
    public string theme { get; set; }
    public Ids ids { get; set; }
    public Tokens tokens { get; set; }
    public string locale { get; set; }
    public string avatarPATH { get; set; }
    public StatCount statCount { get; set; }
    public List<Fight> fights { get; set; }
} 

public class Ids {
    public string discordId { get; set; } //Does this work?

    public string osuId { get; set; }
}

public class Tokens {
    public string discord_access_token { get; set; }
    public string discord_refresh_token { get; set; }
}

public class Fight {
    public string game { get; set; }
    public string id { get; set; }
    public long timestamp { get; set; }
}

public class User {
    public string locale = "us-en";
    public string username;
    public bool admin = false;
    public Waifu waifu = Waifu.FromPoco(DBUtils.GetWaifu("1"));
    public string Id = UserId.CreateId();
    public Ids ids;
    public List<Fight> fights;
    public Tokens tokens;
    public string avatarPATH;
    public StatCount statCount = new StatCount();
    private string theme = "dark_theme";
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
            waifu = waifu.ToPoco(), 
            Id = Id,
            theme = theme,
            ids = ids,
            avatarPATH = avatarPATH,
            locale = locale,
            statCount = statCount,
            fights = fights,
        };
    }
    public PocoUser ToPocoServer(){
        Console.WriteLine("UUUSER RR ***************************");


        var poco = this.ToPoco();

        Console.WriteLine(JsonConvert.SerializeObject(poco));

        poco.tokens.discord_access_token = "";
        poco.tokens.discord_refresh_token = "";
        return poco;
    }
    public static User FromPoco(PocoUser poco)
    {
        User user  = new User(poco.username, poco.ids);
        user.Id = poco.Id;
        user.waifu = Waifu.FromPoco(poco.waifu);
        user.theme = poco.theme;
        user.ids = poco.ids;
        user.avatarPATH = poco.avatarPATH;
        user.locale = poco.locale;
        user.statCount = poco.statCount;
        user.admin = poco.admin;
        user.fights = poco.fights;
        user.tokens = poco.tokens;
        return user;
    }

}