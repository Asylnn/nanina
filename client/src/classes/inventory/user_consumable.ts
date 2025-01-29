import type UserModifier from "./modifiers/user_modifier";
import Item from "./item";
import ItemType from "./item_type";

export default class UserConsumable extends Item {
    public modifiers : UserModifier[] = [];
    public type : ItemType = ItemType.UserConsumable
}