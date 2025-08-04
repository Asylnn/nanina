namespace Nanina.Crafting
{
    public class CraftIngredient
    {
        public ushort id {get; set;}
        public ushort quantity {get; set;}
        public void Test()
        {
            Console.WriteLine("TEST IS FLAWLESS"); 
            Console.WriteLine(quantity); 
        }
    }
    public class Craft
    {
        public ushort id {get; set;}
        public uint moneyCost {get; set;}
        public uint timeCost {get; set;}
        public List<CraftIngredient> ingredients { get; set; } = [];
        public List<CraftIngredient> results {get; set;} = [];
    }
}