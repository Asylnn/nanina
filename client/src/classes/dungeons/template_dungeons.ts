import BossResistances from "./boss_resistances";

export default class DungeonTemplate {
    public id = "";
    public numberOfRewards = 0;
    public bossResistances = new BossResistances;
    public setRewards : number[] = []; 
    public maxHealthByFloor = [Infinity, Infinity, Infinity, Infinity, Infinity]
    public difficulty = 0
    public game_playstyle = 0
}
