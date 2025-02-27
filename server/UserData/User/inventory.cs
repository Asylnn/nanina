using Nanina.UserData.ItemData;

namespace Nanina.UserData
{
    public class Inventory 
    {
        public uint inventoryIdCounter {get; set;} = 0;
        public List<Equipment> equipment {get; set;} = [];
        public List<Item> material {get; set;} = [];
        public List<Item> userConsumable {get; set;} = [];
        public List<Item> waifuConsumable {get; set;} = [];

        public void AddItem(Item item)
        {
            item.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            switch(item.type)
            {
                case ItemType.UserConsumable:
                    var index = userConsumable.FindIndex(UCitem => UCitem.id == item.id);
                    if(index == -1)
                        userConsumable.Add(item);
                    else 
                        userConsumable[index].count += item.count;
                    break;
                case ItemType.WaifuConsumable:
                    break;
                case ItemType.Material:
                    var UCindex = material.FindIndex(material => material.id == item.id);
                        if(UCindex == -1)
                            material.Add(item);
                        else 
                            material[UCindex].count += item.count;
                    break;
            }
        }

        public void AddEquipment(Equipment obtainedEquipment) 
        {
            
            obtainedEquipment.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            equipment.Add(obtainedEquipment);
        }
        public void AddMaterial(Item obtainedMaterial)
        {
            obtainedMaterial.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            var index = material.FindIndex(item => obtainedMaterial.id == item.id);
            if(index == -1)
                material.Add(obtainedMaterial);
            else 
                material[index].count += obtainedMaterial.count;
        }

        public void AddUserConsumable(Item obtainedItem)
        {
            obtainedItem.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            var index = userConsumable.FindIndex(item => obtainedItem.id == item.id);
            if(index == -1)
                userConsumable.Add(obtainedItem);
            else 
                userConsumable[index].count += obtainedItem.count;
        }
    }
}