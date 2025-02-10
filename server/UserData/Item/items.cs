namespace Nanina.UserData.ItemData
{
    public enum ItemType 
    {
        Equipment,
        UserConsumable,
        WaifuConsumable,
        Material,
    }
    public abstract class Item : ICloneable
    {
        public uint count {get; set;}
        public ushort id {get; set;}
        public ItemType type {get; set;}
        public string imgPATH {get; set;}
        public string name {get; set;}
        public string description {get; set;}
        public byte rarity {get; set;}

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}