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
        public long timeout {get; set;}
        public double amount {get; set;}
    }
}