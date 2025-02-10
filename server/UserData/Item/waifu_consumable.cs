using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public class WaifuConsumable : Item
    {
        public new ItemType type {get; set;} = ItemType.WaifuConsumable;
        public WaifuModifier[] modifiers {get; set;}
    }

}