import type WaifuModifier from "../modifiers/waifu_modifier";
import Item from "./item";

export default class Equipement extends Item{
    public kaw_flat! : number;
    public kaw_percent! : number;
    public int_flat! : number;
    public int_percent! : number;
    public agi_flat! : number;
    public agi_percent! : number;
    public luck_flat! : number;
    public luck_percent! : number;
    public str_flat! : number;
    public str_percent! : number;
    public dex_flat! : number;
    public dex_percent! : number;
    public set_id! : number;
    public set_name! : string;
    public modifiers! : WaifuModifier[] ;
    public set_modifiers! : WaifuModifier[];
}