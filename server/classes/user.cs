using System;
using System.Runtime;
using System.Runtime.CompilerServices;

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
    //private Theme theme;

}

public class Ids {
    public string discordId;
}

public class User {
    public string username;
    public Waifu waifu;
    public string userId;
    public Ids ids;
    private string theme; //Used only in client
    public User(string username)
    {
        userId = UserId.CreateId();
        this.username = username;
        this.waifu = new Waifu("Rem");
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
    public static User FromPoco(PocoUser poco, bool forClient = false)
    {
        User user  = new User(poco.username);
        user.userId = poco.userId;
        user.waifu = Waifu.FromPoco(poco.waifu);
        user.theme = poco.theme;
        user.ids = poco.ids;
        return user;
    }
}