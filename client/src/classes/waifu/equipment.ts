import Equipment from "../item/equipment";
import Set from '../item/set'

export default class WaifuEquipmentManager
{
    public weapon !: Equipment | null
    public dress !: Equipment | null
    public accessory !: Equipment | null
    public set !: Set | null

    constructor(obj : WaifuEquipmentManager){
        console.log(obj)
        Object.assign(this, obj)
        //This condition is because we want undefined object when it's not equiped, and not a default object.
        if(this.weapon) this.weapon = Object.assign(new Equipment(),this.weapon )
        if(this.dress) this.dress = Object.assign(new Equipment(),this.dress )
        if(this.accessory) this.accessory = Object.assign(new Equipment(),this.accessory )
    }
}