import type WaifuModifier from "../modifiers/waifu_modifier";
import Item from "./item";

export default class WaifuConsumable extends Item {
    public modifiers! : WaifuModifier[];
}