using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public class UserConsumable : Item
    {
        public new ItemType type = ItemType.UserConsumable;
        public UserModifier[] modifiers {get; set;}
    }
}