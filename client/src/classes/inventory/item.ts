import type ItemType from "./item_type";

export default abstract class Item {
    public count : number = 1;
    public id : number = 0;
    public imgPATH : string = "img path";
    public name : string = "item name";
    public description : string = "item description";
    public type !: ItemType
}