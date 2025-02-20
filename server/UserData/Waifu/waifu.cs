/*
Force -> Attaque physique, Minage de minerais
Agilité -> Vitesse d'attaque, Explorations
Intelligence -> Attaque magique, Decryptage
Chance -> (les % des raretés) Loots, Fights
Dextérité -> Coups critiques, Analyse
Kawaii -> Attaque psychique, Maid café
*/

using LiteDB;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.UserData.WaifuData
{
    public class Waifu
    {
        public string name { get; set; }
        public uint xp { get; set; } = 0;
        public byte lvl { get; set; } = 1;
        public float diffLvlUp { get; set; }
        public string id { get; set; }
        public string imgPATH { get; set; }
        public WaifuEquipmentManager equipment { get; set; } = new ();
        public ushort o_str { get; set; }
        public ushort u_str { get; set; }
        public uint b_str { get; set; }
        public ushort o_luck { get; set; }
        public ushort u_luck { get; set; }
        public uint b_luck { get; set; }
        public ushort o_kaw { get; set; }
        public ushort u_kaw { get; set; }
        public uint b_kaw { get; set; }
        public ushort o_int { get; set; }
        public ushort u_int { get; set; }
        public uint b_int { get; set; }
        public uint b_agi { get; set; }
        public ushort o_agi { get; set; }
        public ushort u_agi { get; set; }
        public uint b_dex { get; set; }
        public ushort o_dex { get; set; }
        public ushort u_dex { get; set; }
        public float Dex {
            get => b_dex*GetMultModificators(StatModifier.DEX); 
        }
        public float Int {
            get => b_int*GetMultModificators(StatModifier.INT); 
        }
        public float Agi {
            get => b_agi*GetMultModificators(StatModifier.AGI); 
        }
        public float Str {
            get => b_str*GetMultModificators(StatModifier.STR); 
        }
        public float Kaw {
            get => b_kaw*GetMultModificators(StatModifier.KAW); 
        }
        public float Luck {
            get => b_luck*GetMultModificators(StatModifier.LUCK); 
        }
        public float Psychic {
            get => 2*Kaw*GetMultModificators(StatModifier.Psychic); 
        }
        public float Magical {
            get => 2*Int*GetMultModificators(StatModifier.Magical); 
        }
        public float Physical {
            get => 2*Str*GetMultModificators(StatModifier.Physical); 
        }
        public float CritChance {
            get => (0.05f + Luck/10)*GetMultModificators(StatModifier.CritDamage); 
        }
        public float CritDamage {
            get => (0.50f + Dex/20)*GetMultModificators(StatModifier.CritChance); 
        }
        public byte stars { get; set; }
        private uint XpToLvlUp
        {
            get => (uint) Math.Floor(diffLvlUp * (10 * lvl + 20));
        }

        public void GiveXP(uint _xp)
        {
            xp += _xp;
            if(xp >= XpToLvlUp){
                xp -= XpToLvlUp;
                uint temp_xp = xp;
                LevelUp();
                xp = 0;
                GiveXP(temp_xp);
            }
        }
        public void LevelUp()
        {
            lvl++;
            b_str += o_str;
            b_agi += o_agi;
            b_kaw += o_kaw;
            b_int += o_int;
            b_dex += o_dex;
            b_luck += o_luck;
        }
        public void Update(){
            var DBwaifu = DBUtils.GetWaifu(id);
            diffLvlUp = DBwaifu.diffLvlUp;
            imgPATH = DBwaifu.imgPATH;
            name = DBwaifu.name;
            o_str = DBwaifu.o_str;
            u_str = DBwaifu.u_str;
            b_str = o_str + (lvl-1u)*u_str;
            o_kaw = DBwaifu.o_kaw;
            u_kaw = DBwaifu.u_kaw;
            b_kaw = o_kaw + (lvl-1u)*u_kaw;
            o_dex = DBwaifu.o_dex;
            u_dex = DBwaifu.u_dex;
            b_dex = o_dex + (lvl-1u)*u_dex;
            o_agi = DBwaifu.o_agi;
            u_agi = DBwaifu.u_agi;
            b_agi = o_agi + (lvl-1u)*u_agi;
            o_int = DBwaifu.o_int;
            u_int = DBwaifu.u_int;
            b_int = o_int + (lvl-1u)*u_int;
            o_luck = DBwaifu.o_luck;
            u_luck = DBwaifu.u_luck;
            b_luck = o_luck + (lvl-1u)*u_luck;
        }

        public Equipment Equip(Equipment newEquipment)
        {
            // Equips the equipment and check for set changes
            Equipment oldEquipment = null;
            switch(newEquipment.piece){
                case EquipmentPiece.Weapon:
                    oldEquipment = equipment.weapon;
                    equipment.weapon = newEquipment;
                    break;
                case EquipmentPiece.Dress:
                    oldEquipment = equipment.dress;
                    equipment.dress = newEquipment;
                    break;
                case EquipmentPiece.Accessory:
                    oldEquipment = equipment.accessory;
                    equipment.accessory = newEquipment;
                    break;
            }
            if(equipment.weapon?.setId == equipment.dress?.setId && equipment.dress?.setId == equipment.accessory?.setId && equipment.dress?.setId != null)
            {

                /*get set col to find one on the equipement to update equipment.set*/
                equipment.set = DBUtils.Get<Set>(set => set.id == equipment.weapon.setId);
            }
            else
            {
                equipment.set = null;
            }
                
            return oldEquipment;
        }

        public float GetMultModificators(StatModifier statModifier){
            //The following line probably doesn't work?
            var modificators = new List<Modifier>().Concat(equipment.weapon?.modifiers ?? []).Concat(equipment.dress?.modifiers ?? []).Concat(equipment.accessory?.modifiers ?? []).Concat(equipment.set?.modifiers ?? []);
            modificators = modificators.Where(modif => modif?.operationType == OperationType.Multiplicative && modif?.stat == statModifier);
            
            return modificators?.Aggregate(1.0f, (amount, modificator) => amount += modificator?.amount ?? 0) ?? 1.0f;
        }
    }
}
