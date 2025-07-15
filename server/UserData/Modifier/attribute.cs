namespace Nanina.UserData.ModifierData
{
    public class EquipmentAttribute 
    {
        public byte tier {get; set;}
        public ushort id {get; set;}
        public List<Modifier> modifiers {get; set;}
    }
}