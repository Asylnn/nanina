namespace Nanina.UserData.ModifierData
{
    public class EquipmentAttribute 
    {
        public short tier {get; set;}
        public short id {get; set;}
        public required List<Modifier> modifiers { get; set; }
    }
}