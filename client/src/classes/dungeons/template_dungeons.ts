import BossResistances from "./boss_resistances";

export default class DungeonTemplate {
    public id !: string
    public numberOfRewardsPerEnergy !: number
    public bossResistances !: BossResistances
    public setRewards !: number[]
    public maxHealthByFloor !: number[]
    public game_playstyle !: number
}