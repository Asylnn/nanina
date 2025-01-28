import type Dictionary from "../dictionary";
import type Equipement from "./equipement";
import type UserConsumable from "./user_consumable";
import type WaifuConsumable from "./waifu_consumable";
import type Materials from "./materials";

export default class Inventory {
    public equipment!: Dictionary<Equipement>;
    public userConsumable!: Dictionary<UserConsumable>;
    public waifuConsumbale!: Dictionary<WaifuConsumable>;
    public materials!: Dictionary<Materials>;
}







