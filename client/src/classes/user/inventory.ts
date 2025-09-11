import type Dictionary from "../dictionary";
import type Equipment from "../item/equipment";
import type Item from "../item/item";


export default class Inventory {
    public equipment!: Dictionary<Equipment>;
    public items!: Dictionary<Item>
    public inventoryIdCounter !: number

    public AddItem(item: Item)
    {
        let oldItem = this.items[item.id]
        if(oldItem == undefined)
        {
            item.inventoryId = this.inventoryIdCounter;
            this.inventoryIdCounter++;
            this.items[item.id] = item;
        }
        else
        {
            this.items[item.id].count += item.count;
        }
    }

    public RemoveItem(item : Item)
    {
        this.RemoveItemWithId(item.id, 1);
    }

    public RemoveItemWithId(id : number, quantity : number)
    {
        let oldItem = this.items[id]
        if(oldItem == undefined)
        {
            console.log("error with removing item with id : ", id)
            return;
        }
        oldItem.count -= quantity;
        if(oldItem.count <= 0)
            delete this.items[id]
        
    }

    public AddEquipment(obtainedEquipment : Equipment) 
    {
        obtainedEquipment.inventoryId = this.inventoryIdCounter;
        this.inventoryIdCounter++;
        this.equipment[obtainedEquipment.inventoryId] = obtainedEquipment;
    }
}







