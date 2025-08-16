namespace Nanina.Crafting
{
    public class CraftIngredient
    {
        public short id {get; set;}
        public short quantity {get; set;}
        public string? imgPATH {get; set;} // Not present in crafting.json, added after deserializing 
    }
    public class Craft
    {
        public short id {get; set;}
        public int moneyCost {get; set;}
        public int timeCost {get; set;}
        public List<CraftIngredient> ingredients { get; set; } = [];
        public List<CraftIngredient> results {get; set;} = [];
    }
}