import StatCount from './stats'
import Waifu from '../waifu/waifu'
import type Dictionary from '../dictionary'
import PullBannerHistory from './pull_history'
import type Fight from './fight'
import type Inventory from './inventory'
import type Game from './game'



export default class User {
    public xp! : number
    public lvl! : number
    public XpToLvlUp! : number
    public admin : boolean = false
    public username : string = "Pro Osu Player"
    public theme : string = "dark_theme"
    public energy!: number
    public max_energy!: number
    public ids : any = {}
    public statCount : StatCount = new StatCount()
    public waifus : Waifu[] = [new Waifu({})]
    public Id : string = "772277"
    public locale : string = "en"
    public avatarPATH : string = ""
    public gacha_currency : number = 0
    public pullBannerHistory: Dictionary<PullBannerHistory> = {}//?
    public localFightTimestamp : number = 0
    public claimTimestamp : number = 0
    public fightHistory : Dictionary<string[]> = {}
    public fight! : Fight
    public inventory! : Inventory
    public lvlRewards! : number
    public preferedGame! : Game

    public get totalClaims() : number{
        return this.statCount.maimai_claim_count + this.statCount.std_claim_count
    }
    
    public verification : any = {} //One day any objects should be properly be typed
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