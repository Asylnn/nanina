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
        public ModifierChance[] modifierChancesWeapon;
        public ModifierChance[] modifierChancesDress;
        public ModifierChance[] modifierChancesAccessory;

    }

    public class ModifierChance {
        public Modifier modifier;
        public uint weight;
    }
}