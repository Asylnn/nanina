import StatCount from './stats'
import Waifu from '../waifu/waifu'
import type Dictionary from '../dictionary'
import PullBannerHistory from './pull_history'
import type Fight from './fight'
import type Inventory from './inventory'
import type Game from './game'
import config from '../../../../baseValues.json'
import Equipment from '../item/equipment'
import type Activity from './activity'
import type Verification from './verification'



export default class User {
    public money!: number
    public xp! : number
    public lvl! : number
    public XpToLvlUp! : number
    public maxConcurrentActivities! :number
    public activities!: Activity[]
    public admin : boolean = false
    public username : string = "Pro Osu Player"
    public theme : string = "dark_theme"
    public energy!: number
    public max_energy!: number
    public ids : any = {}
    public statCount : StatCount = new StatCount()
    public waifus !: Dictionary<Waifu> 
    public get availableWaifus() : Waifu[] {
        return Object.values(this.waifus).filter(waifu => ! waifu.isDoingSomething)
    }
    public Id : string = "772277"
    public locale : string = "en"
    public avatarPATH : string = ""
    public gacha_currency : number = 0
    public pullBannerHistory: Dictionary<PullBannerHistory> = {}//?
    
    public claimTimestamp : number = 0
    public fightHistory : Dictionary<string[]> = {}
    public fight !: Fight | null
    public inventory! : Inventory
    public lvlRewards! : number
    public preferedGame! : Game
    public completedResearches !: Dictionary<number>
    public lastContinuousFightTimestamp !: number
    /*
        Local only properties
    */

    public localFightTimestamp = 0
    public fight_timing_out = false
    public claim_timing_out = false

    public get totalClaims() : number{
        return this.statCount.maimai_claim_count + this.statCount.std_claim_count
    }
    
    public verification !: Verification
    constructor(obj : any){
        console.log("user constructor")
        console.log(obj)
        Object.assign(this, obj)
        for(let waifu in this.waifus)
        {
            this.waifus[waifu] = new Waifu(this.waifus[waifu])
        }
        if(this.inventory) this.inventory.equipment = this.inventory?.equipment.map((equipment) => {
            return Object.assign(new Equipment(), equipment)
        })
        
        if(this.fight?.timestamp != undefined)
            this.localFightTimestamp = this.fight.timestamp
            
        User.updateTimer(this)
        setInterval(User.updateTimer, 1000, this)
    }
    
    isResearchDone(id:string) : boolean
    {
        return Object.keys(this.completedResearches).some(researchId => researchId == id)
    }

    static updateTimer(user: User) {
        let date_milli = Date.now()
        user.fight_timing_out = user.localFightTimestamp + config.time_for_allowing_another_fight_in_milliseconds >= date_milli
        user.claim_timing_out = user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds >= date_milli
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