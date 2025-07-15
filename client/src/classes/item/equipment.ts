import Item from "./item";
import EquipmentPiece from "./piece";
import ItemType from "./item_type";
import Modifier from "../modifiers/modifiers";
import type EquipmentAttribute from "./attribute";

export default class Equipment extends Item {
    public setId : number = 0;
    public set_name : string = "set name";
    public piece : EquipmentPiece = EquipmentPiece.Weapon
    public type : ItemType = ItemType.Equipment
    public stat : Modifier = new Modifier
    public lvl : number = 1
    public attributes : EquipmentAttribute[] = []


    public getAttributeModifiers()
    {
        return this.attributes.reduce((modifiers, attribute) => [... modifiers, ... attribute.modifiers], new Array<Modifier>());
    }
    public  GetAllModifiers() : Array<Modifier>
    {
        return [this.stat, ... this.modifiers, ...this.getAttributeModifiers()];
    }

}