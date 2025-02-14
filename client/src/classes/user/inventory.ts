import type Dictionary from "../dictionary";
import type Equipment from "../item/equipment";
import type Item from "../item/item";


export default class Inventory {
    public equipment!: Array<Equipment>;
    public userConsumable!: Array<Item>;
    public waifuConsumable!: Array<Item>;
    public material!: Array<Item>;
}







