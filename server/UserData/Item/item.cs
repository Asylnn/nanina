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
    public class Item : ICloneable
    {
        public ushort id {get; set;}
        public uint count {get; set;}
        public ItemType type {get; set;}
        public string imgPATH {get; set;}
        public byte rarity {get; set;}
        public List<Modifier> modifiers {get; set;}
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public override List<Modifier> GetAllModifiers()
        {
            return modifiers;
        }
    }
}