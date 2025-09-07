import Modifier from "../modifiers/modifiers"
import StatModifier from "../modifiers/stat_modifier"
import WaifuEquipmentManager from "./equipment"
import OperationType from "../modifiers/operation_type"

export default class Waifu {
    public id : string = "42"
    public xp : number = 0
    public lvl : number = 1
    public diffLvlUp : number = 3
    public imgPATH : string = "GYrXGACboAACxp7.jpg"
    public stars : number = 2
    public equipment! : WaifuEquipmentManager
    public isDoingSomething! : Boolean

    public b_str : number = 0
    public o_str : number = 0
    public u_str : number = 0
    public b_agi : number = 0
    public o_agi : number = 0
    public u_agi : number = 0
    public b_kaw : number = 0
    public o_kaw : number = 0
    public u_kaw : number = 0
    public b_dex : number = 0
    public o_dex : number = 0
    public u_dex : number = 0
    public b_luck : number = 0
    public o_luck : number = 0
    public u_luck : number = 0
    public b_int : number = 0
    public o_int : number = 0
    public u_int : number = 0
    
    public get dex() : number {
        return this.ApplyModificators(this.b_dex, StatModifier.DEX);
    }
    public get int() : number {
        return this.ApplyModificators(this.b_int, StatModifier.INT);
    }
    public get agi() : number {
        return this.ApplyModificators(this.b_agi, StatModifier.AGI); 
    }
    public get str() : number {
        return this.ApplyModificators(this.b_str, StatModifier.STR); 
    }
    public get kaw() : number {
        return this.ApplyModificators(this.b_kaw, StatModifier.KAW); 
    }
    public get luck() : number {
        return this.ApplyModificators(this.b_luck, StatModifier.LUCK); 
    }
    public get psychic() : number {
        return this.ApplyModificators(2*this.kaw, StatModifier.Psychic); 
    }
    public get magical() : number {
        return this.ApplyModificators(2*this.int, StatModifier.Magical); 
    }
    public get physical() : number {
        return this.ApplyModificators(2*this.str, StatModifier.Physical); 
    }
    public get critChance() : number {
        return this.ApplyModificators(0.05 + this.agi/400, StatModifier.CritChance); 
    }
    public get critDamage() : number {
        return this.ApplyModificators(0.50 + this.dex/400, StatModifier.CritDamage); 
    }

    //public XpToLvlUp! : number

    public get XpToLvlUp() : number {
        return Math.ceil(50 + 10*this.lvl + 0.5*Math.pow(this.lvl, 2));
    }

    public get points() : number {
        return this.o_str + this.o_kaw + this.o_int + 99*(this.u_kaw + this.u_str + this.u_int) + 2*(this.o_agi + this.o_dex + 99*(this.u_agi + this.u_dex)) + 4*(this.o_luck + 99*this.u_luck)
    }

    constructor(obj: any){
        console.log(obj)
        Object.assign(this, obj)
        this.equipment = new WaifuEquipmentManager(this.equipment);
    }

    public GetMultModificators(statModifier : StatModifier){
        this.equipment.weapon?.GetAllModifiers()
        return ([] as Array<Modifier | undefined>).concat(this.equipment.weapon?.GetAllModifiers() || []).concat(this.equipment.dress?.GetAllModifiers() || []).concat(this.equipment.accessory?.GetAllModifiers() || []).concat(this.equipment.set?.modifiers || [])
            .filter(modif => modif?.operationType == OperationType.Multiplicative && modif?.stat == statModifier)
            .reduce((amount, modificator) => amount += modificator?.amount || 0, 1) || 1;
    }

    public GetAdditiveModificators(statModifier: StatModifier){
        return ([] as Array<Modifier | undefined>).concat(this.equipment.weapon?.GetAllModifiers() || []).concat(this.equipment.dress?.GetAllModifiers() || []).concat(this.equipment.accessory?.GetAllModifiers() || []).concat(this.equipment.set?.modifiers || [])
            .filter(modif => modif?.operationType == OperationType.Additive && modif?.stat == statModifier)  
            .reduce((amount, modificator) => amount += modificator?.amount || 0, 0) || 0;
    }

    public DisplayMultModificator(statModifier : StatModifier) : string
    {
        let modificatorAmount = this.GetMultModificators(statModifier) - 1
        modificatorAmount = Math.floor(modificatorAmount*1000)/10
        return `+${modificatorAmount}%`
    }

    public DisplayAdditiveModificator(statModifier : StatModifier) : string
    {
        return `+${Math.floor(this.GetAdditiveModificators(statModifier))}`
    }
    public ApplyModificators(stat : number, statModifier: StatModifier) : number
    {
        return (stat + this.GetAdditiveModificators(statModifier))*this.GetMultModificators(statModifier);
    }
        

    /*constructor(name :string ,xp : number,lvl:number,diffLvlUp : number,imgPATH : string) {
        this.name = name
        this.xp = xp
        this.diffLvlUp = diffLvlUp
        this.lvl = lvl
        this.imgPATH = imgPATH
    }*/
}