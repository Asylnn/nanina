import type Dictionary from "../dictionary";
import type Equipment from "./equipment";
import type UserConsumable from "./user_consumable";
import type WaifuConsumable from "./waifu_consumable";
import type Material from "./material";

export default class Inventory {
    public equipmentCount! : number;
    public equipment!: Dictionary<Equipment>;
    public userConsumable!: Dictionary<UserConsumable>;
    public waifuConsumable!: Dictionary<WaifuConsumable>;
    public material!: Dictionary<Material>;
}







