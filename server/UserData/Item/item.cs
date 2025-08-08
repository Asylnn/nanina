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
        public ushort id {get; set;}
        public uint inventoryId {get; set;}
        public uint count {get; set;}
        public ItemType type {get; set;}
        public required string imgPATH {get; set;}
        public byte rarity {get; set;}
        public required List<Modifier> modifiers {get; set;}
    }
}