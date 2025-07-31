using Nanina.UserData.ModifierData;

public class ResearchNode
{
    public byte tier;
    public string id;
    public string[] requirements;
    public List<Modifier> modifiers;
    public bool infinite;
    public double cost;
}