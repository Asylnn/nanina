using Nanina.UserData.ModifierData;

namespace Nanina.UserData.ItemData
{
    public class Set
    {
        public short id {get; set;}
        public required Modifier[] modifiers {get; set;}
    }
}