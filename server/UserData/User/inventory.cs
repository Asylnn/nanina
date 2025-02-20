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

        public void AddEquipment(Equipment obtainedEquipment) 
        {
            
            obtainedEquipment.inventoryId = inventoryIdCounter;
            inventoryIdCounter++;
            //Console.WriteLine(inventoryIdCounter);
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
    }
}