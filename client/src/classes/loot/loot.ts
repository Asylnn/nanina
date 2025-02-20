import type Equipment from "../item/equipment"
import type Item from "../item/item"
import type LootType from "./loot_type"

export default class Loot
{
    public lootType! : LootType
    public amount! : number
    public item! : Item | Equipment | null
}