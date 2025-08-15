using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public enum ItemType 
    {
        Equipment,
        UserConsumable,
        WaifuConsumable,
        Material,
    }
    public class Item
    {
        public short id {get; set;}
        public int inventoryId {get; set;}
        public int count {get; set;}
        public ItemType type {get; set;}
        public required string imgPATH {get; set;}
        public short rarity {get; set;}
        public required List<Modifier> modifiers {get; set;}
    }
}