import Item from "./item";
import EquipmentPiece from "./piece";
import ItemType from "./item_type";
import Modifier from "../modifiers/modifiers";

export default class Equipment extends Item{
    public setId : number = 0;
    public set_name : string = "set name";
    public piece : EquipmentPiece = EquipmentPiece.Weapon
    public type : ItemType = ItemType.Equipment
    public stat! : Modifier
}