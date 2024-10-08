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

public class PocoUser
{
    public string username { get; set; } 
    public PocoWaifu waifu { get; set; }
    public string userId { get; set; }
    public string theme { get; set; }
    public Ids ids { get; set; }
    public Tokens tokens { get; set; }
}

public class Ids {
    public string discordId { get; set; } //Does this work?
}

public class Tokens {
    public string discord_access_token { get; set; }
    public string discord_refresh_token { get; set; }
}

public class User {
    public string username;
    public Waifu waifu;
    public string userId;
    public Ids ids;
    public Tokens tokens;
    private string theme; //Used only in client
    public User(string username, Ids ids)
    {
        userId = UserId.CreateId();
        this.username = username;
        this.waifu = new Waifu("Rem", "src/assets/waifu-image/GYrXGACboAACxp7.jpg");
        this.ids = ids;
        theme = "dark_theme";
    }
    public PocoUser ToPoco()
    {
        return new PocoUser
        {
            username = username,
            waifu = waifu.ToPoco(),
            userId = userId,
            theme = theme,
            ids = ids
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
        user.userId = poco.userId;
        user.waifu = Waifu.FromPoco(poco.waifu);
        user.theme = poco.theme;
        user.ids = poco.ids;
        return user;
    }

}