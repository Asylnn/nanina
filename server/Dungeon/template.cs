using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;

namespace Nanina.Dungeon
{
    public class Template {
        public string id;
        public byte numberOfRewards;
        public BossResistances bossResistances;
        public ushort[] setRewards; 
        public float maxHealth;
        public byte difficulty;
        public ModifierWeights[] modifierWeightsWeapon;
        public ModifierWeights[] modifierWeightsDress;
        public ModifierWeights[] modifierWeightsAccessory;

    }

    public class ModifierWeights {
        public Modifier modifier;
        public uint weight;
    }
}