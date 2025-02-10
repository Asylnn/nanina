using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public enum EquipmentPiece {
        Weapon,
        Dress,
        Accessory,
    }


    public class Set
    {
        public string id;
        public WaifuModifier[] modifiers;
    }

    public class Equipment : Item
    {
        public new ItemType type {get; set;} = ItemType.Equipment;
        /*public int kaw_flat;
        public int kaw_percent;
        public int int_flat;
        public int int_percent;
        public int agi_flat;
        public int agi_percent;
        public int luck_flat;
        public int luck_percent;
        public int str_flat;
        public int str_percent;
        public int dex_flat;
        public int dex_percent;*/
        public uint equipmentId {get; set;}
        public ushort setId {get; set;}
        public string set_name {get; set;}
        public EquipmentPiece piece {get; set;}
        public WaifuModifier stat {get; set;}
        public WaifuModifier[] modifiers {get; set;}
        public WaifuModifier[] setModifiers {get; set;}

        public WaifuModifier[] GetAllModifiers()
        {
            return (WaifuModifier[]) modifiers.Concat(setModifiers).Append(stat);
        }
    }
}