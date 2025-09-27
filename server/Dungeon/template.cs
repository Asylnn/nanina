using Nanina.Osu;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;

namespace Nanina.Dungeon
{
    public class Template {
        public required string id;
        public byte numberOfRewardsPerEnergy;
        public required BossResistances bossResistances;
        public required ushort[] setRewards; 
        public required double[] maxHealthByFloor;
        public NaninaStdTag game_playstyle;
    }

    public class ModifierWeights {
        public required Modifier modifier;
        public uint weight;
    }
}