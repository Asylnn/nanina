using Nanina.UserData.ItemData;

namespace Nanina.UserData
{
    public class Inventory 
    {
        public List<Equipment> equipment {get; set;} = [];
        public List<Item> material {get; set;} = [];
        public List<Item> userConsumable {get; set;} = [];
        public List<Item> waifuConsumable {get; set;} = [];
    }
}