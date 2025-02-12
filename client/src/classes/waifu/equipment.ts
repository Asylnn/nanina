import type Equipment from "../item/equipment";
import Set from '../item/set'

export default interface WaifuEquipmentManager
{
    weapon : Equipment
    dress : Equipment
    accessory : Equipment
    set : Set
}