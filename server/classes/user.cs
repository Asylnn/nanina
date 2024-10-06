using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

enum Theme {
    DarkTheme,
    PinkTheme,
    WhiteTheme
}


static class UserId {
    static public string CreateId()
    {   
        Random rng = new Random();
        return Utils.GetTimestamp() + (string) rng.Next(10000000).ToString();
    }
}

class PocoUser
{
    public string username { get; set; } 
    public PocoWaifu waifu { get; set; }
    public string userId { get; set; }
    //private Theme theme;

}

class User {
    public string username;
    public Waifu waifu;
    public string userId;
    private Theme theme;
    public User(string _username)
    {
        userId = UserId.CreateId();
        username = _username;
        waifu = new Waifu("Rem", userId);
        theme = Theme.DarkTheme;
    }
    public PocoUser ToPoco()
    {
        return new PocoUser
        {
            username = username,
            waifu = waifu.ToPoco(),
            userId = userId,
            //Theme = theme
        };
    }

}