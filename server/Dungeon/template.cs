using Nanina.Osu;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;

namespace Nanina.Dungeon
{
    public class Template {
        public string id;
        public byte numberOfRewardsPerEnergy;
        public BossResistances bossResistances;
        public ushort[] setRewards; 
        public float[] maxHealthByFloor;
        public byte difficulty;
        public NaninaStdTag game_playstyle;
        public ModifierWeights[] modifierWeightsWeapon;
        public ModifierWeights[] modifierWeightsDress;
        public ModifierWeights[] modifierWeightsAccessory;

    }

    public class ModifierWeights {
        public Modifier modifier;
        public uint weight;
    }
}