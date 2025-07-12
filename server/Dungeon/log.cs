using System.Reflection.Metadata.Ecma335;

namespace Nanina.Dungeon
{
    public class DungeonLog 
    {
        public string waifuId;
        public string attackType;
        public double dmg;
        public byte critical_amount; //if critting, critical_amount = 1, if super critting, critical_amount = 2 etc...
        public bool claim_attack;
    }
}