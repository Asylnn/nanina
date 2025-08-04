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
        public List<Item> AllItems 
        {
            get => [.. userConsumable, .. waifuConsumable, .. material];
        }

        public bool HasItem(ushort id, ushort quantity = 1)
        {
            return AllItems.Any(item => item.id == id && item.count >= quantity);
        }
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

        public void RemoveItem(Item item)
        {
            switch(item.type)
            {
                case ItemType.UserConsumable:
                    if(item.count == 1)
                        userConsumable.Remove(item);
                    else 
                        item.count--;
                    break;
                case ItemType.WaifuConsumable:
                    if(item.count == 1)
                        waifuConsumable.Remove(item);
                    else 
                        item.count--;
                    break;
                case ItemType.Material:
                    if(item.count == 1)
                        material.Remove(item);
                    else 
                        item.count--;
                    break;
            }
        }

        public void RemoveItem(ushort id, ushort quantity)
        {
            var item = AllItems.Find(item => item.id == id);
            if(item.count > quantity)
            {
                item.count -= quantity;
            }
            else
            {
                item.count = 1;
                RemoveItem(item);
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