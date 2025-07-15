using Nanina.UserData.ItemData;

namespace Nanina.Communication
{
    public enum LootType
    {
        UserXP,
        WaifuXP,
        GC,
        Item,
        Equipment
    }
    public class Loot
    {
        public LootType lootType;
        public uint amount;
        public Item item;
    }

    public class LiteLoot
    {
        public LootType lootType;
        public uint amount;
        public ushort? itemId;
    }
}