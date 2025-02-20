import ItemType from "./item_type";
import Modifier from '../modifiers/modifiers'

export default class Item {
    public inventoryId! : number
    public count : number = 1;
    public id : number = 0;
    public imgPATH : string = "img path";
    public name : string = "item name";
    public description : string = "item description";
    public type : ItemType = ItemType.Material
    public modifiers : Modifier[] = []
    public rarity : number = 1
}