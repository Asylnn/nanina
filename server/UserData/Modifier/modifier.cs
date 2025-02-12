namespace Nanina.UserData.ModifierData
{
    public enum OperationType
    {
        Additive,
        Multiplicative,
        
    }
    public class Modifier 
    {
        public OperationType operationType {get; set;}
        public StatModifier stat {get; set;}
        public ulong timeout {get; set;}
        public float amount {get; set;}
    }
}