using Nanina.UserData.ItemData;
using Nanina.UserData;
using Nanina.Database;

namespace Nanina.Communication
{
    public enum LootType
    {
        UserXP,
        WaifuXP,
        Money,
        GC,
        Item,
        Equipment,
        Modifiers,
        TimeSave,
    }
    public class Loot
    {
        public LootType lootType { get; set; }
        public int amount { get; set; }
        public Item? item { get; set; }

        public static void GrantLoot(List<Loot> loots, User user)
        {
            foreach(var loot in loots)
            {
                switch(loot.lootType){
                    case LootType.GC:
                        user.gacha_currency += loot.amount;
                        break;
                    case LootType.Money:
                        user.money += loot.amount;
                        break;
                    case LootType.Item:
                        if(loot.amount != 1)
                            loot.item!.count = loot.amount;
                        user.inventory.AddItem(loot.item!);
                        break;
                    case LootType.Modifiers:
                        user.UseUserConsumable(loot.item!);
                        break;
                }
            }
        }
    }

    public class LiteLoot
    {
        public LootType lootType;
        public int amount;
        public short itemId = -1;
    }
}