import type Loot from "../loot/loot"
import type ActivityType from "./activity_type"

export default class Activity
{
    public id! : number
    public timerId! : number
    public type!: ActivityType
    public timestamp!:number
    public timeout!:number
    public waifuID!:string
    public loot!: Loot[]
    public finished!: boolean
}

