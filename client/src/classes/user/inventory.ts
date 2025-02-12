import type Dictionary from "../dictionary";
import type Equipment from "../item/equipment";
import type Item from "../item/item";


export default class Inventory {
    public equipment!: Dictionary<Equipment>;
    public userConsumable!: Dictionary<Item>;
    public waifuConsumable!: Dictionary<Item>;
    public material!: Dictionary<Item>;
}







