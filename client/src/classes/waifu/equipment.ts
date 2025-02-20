import type Equipment from "../item/equipment";
import Set from '../item/set'

export default interface WaifuEquipmentManager
{
    weapon : Equipment | null
    dress : Equipment | null
    accessory : Equipment | null
    set : Set | null
}