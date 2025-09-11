using Nanina.UserData.ItemData;

namespace Nanina.UserData
{
    public class Inventory 
    {
        public int inventoryIdCounter {get; set;} = 0;
        public Dictionary<int, Equipment> equipment {get; set;} = []; //use inventory id as key
        public Dictionary<short, Item> items {get; set;} = [];          //use item id as key
        public void AddItem(Item item)
        {
            
            if(items.TryGet(item.id, out var oldItem) == false)
            {
                item.inventoryId = inventoryIdCounter;
                inventoryIdCounter++;
                items[item.id] = item;
            }
            else
            {
                items[oldItem.id].count += item.count;
            }
        }

        public void RemoveItem(Item item)
        {
            RemoveItem(item.id, 1);
        }

        public void RemoveItem(short id, int quantity)
        {
            if(items.TryGet(id, out var item) == false)
            {
                Console.Error.WriteLine($"Remove Item got invalid id: {id}");
                return;
            }
            item.count -= quantity;
            if(item.count <= 0)
                items.Remove(item.id);
            
        }

        public void AddEquipment(Equipment obtainedEquipment) 
        {
            
            obtainedEquipment.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            equipment[obtainedEquipment.inventoryId] = obtainedEquipment;
        }
    }
}