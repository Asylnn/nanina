using System.Xml.XPath;
using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public enum EquipmentPiece {
        Weapon,
        Dress,
        Accessory,
    }


    
    public class Equipment : Item
    {
        public new ItemType type {get; set;} = ItemType.Equipment;
        public Set set {get; set;}
        public EquipmentPiece piece {get; set;}
        public new List<Modifier> GetAllModifiers()
        {
            return (List<Modifier>) modifiers.Concat(set.modifiers);
        }
    }
}