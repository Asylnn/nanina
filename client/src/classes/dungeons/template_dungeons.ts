import BossResistances from "./boss_resistances";

export default class DungeonTemplate {
    public id = "";
    public numberOfRewards = 0;
    public bossResistances = new BossResistances;
    public setRewards : number[] = []; 
    public maxHealth = Infinity
    public difficulty = 0
}
