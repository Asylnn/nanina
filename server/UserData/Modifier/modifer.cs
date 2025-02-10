namespace Nanina.UserData.ModifierData
{
    public abstract class Modifier 
    {
        public ushort id {get; set;}
        public ulong timeout {get; set;}
        public float amount {get; set;}
    }

}