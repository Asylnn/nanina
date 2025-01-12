import { StatCount } from './stats'
import Waifu from './waifu'

export default class User {
    public username : string = "Pro Osu Player"
    public theme : string = "dark_theme"
    public ids : any = {}
    public statCount : StatCount = new StatCount()
    public waifu : Waifu = new Waifu()
    public Id : string = "772277"
    public locale : string = "en"
    public avatarPATH = ""
}



/*
public class PocoUser
{
    public string username { get; set; } 
    public PocoWaifu waifu { get; set; }
    public string userId { get; set; }
    public string theme { get; set; }
    public Ids ids { get; set; }
    --> public Tokens tokens { get; set; } This one is not sent to the client
    public string locale { get; set; }
    public string avatarPATH { get; set; }
    public StatCount statCount { get; set; }
}
    */