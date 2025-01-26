import { StatCount } from './stats'
import Waifu from './waifu'
import type Dictionary from './dictionary'
import PullBannerHistory from './pull_history'

export default class User {
    public admin : boolean = false
    public username : string = "Pro Osu Player"
    public theme : string = "dark_theme"
    public ids : any = {}
    public statCount : StatCount = new StatCount()
    public waifus : Waifu[] = [new Waifu({})]
    public Id : string = "772277"
    public locale : string = "en"
    public avatarPATH = ""
    public gacha_currency = 0
    public pullBannerHistory: Dictionary<PullBannerHistory> = {}//?
    

    constructor(obj : any){
        Object.assign(this, obj)
        this.waifus.forEach(waifu => Object.assign(new Waifu({}), waifu))
    }
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