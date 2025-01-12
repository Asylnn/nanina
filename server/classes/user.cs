using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Principal;

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
}

public class Ids {
    public string discordId { get; set; } //Does this work?

    public string osuId { get; set; }
}

public class Tokens {
    public string discord_access_token { get; set; }
    public string discord_refresh_token { get; set; }
}

public class User {
    public string locale = "us-en";
    public string username;
    public bool admin = false;
    public Waifu waifu = new Waifu("Rem", "src/assets/waifu-image/GYrXGACboAACxp7.jpg");
    public string Id = UserId.CreateId();
    public Ids ids;
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
            waifu = waifu.ToPoco(),
            Id = Id,
            theme = theme,
            ids = ids,
            avatarPATH = avatarPATH,
            locale = locale,
            statCount = statCount,
        };
    }
    public PocoUser ToPocoServer(){
        var poco = this.ToPoco();
        poco.tokens.discord_access_token = "";
        poco.tokens.discord_refresh_token = "";
        return poco;
    }
    public static User FromPoco(PocoUser poco, bool forClient = false)
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
        return user;
    }

}