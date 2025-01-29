import type WaifuModifier from "./modifiers/waifu_modifier";
import Item from "./item";
import ItemType from "./item_type";

export default class WaifuConsumable extends Item {
    public modifiers : WaifuModifier[] = [];
    public type : ItemType = ItemType.WaifuConsumable
}