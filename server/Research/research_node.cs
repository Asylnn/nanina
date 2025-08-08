using Nanina.UserData.ModifierData;

public class ResearchNode
{
    public byte tier;
    public required string id;
    public required string[] requirements;
    public required List<Modifier> modifiers;
    public bool infinite;
    public double cost;
}