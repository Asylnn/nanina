using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;

namespace Nanina.UserData.WaifuData
{
    public class Waifu
    {
        public int xp { get; set; } = 0;
        public short lvl { get; set; } = 1;
        public float diffLvlUp { get; set; }
        public required string id { get; set; }
        public required string imgPATH { get; set; }
        public WaifuEquipmentManager equipment { get; set; } = new ();
        public bool isDoingSomething { get; set; } = false;
        public short o_str { get; set; }
        public short u_str { get; set; }
        public int b_str { get; set; }
        public short o_luck { get; set; }
        public short u_luck { get; set; }
        public int b_luck { get; set; }
        public short o_kaw { get; set; }
        public short u_kaw { get; set; }
        public int b_kaw { get; set; }
        public short o_int { get; set; }
        public short u_int { get; set; }
        public int b_int { get; set; }
        public int b_agi { get; set; }
        public short o_agi { get; set; }
        public short u_agi { get; set; }
        public int b_dex { get; set; }
        public short o_dex { get; set; }
        public short u_dex { get; set; }
        public double Dex {
            get => ApplyModificators(b_dex, StatModifier.DEX);
        }
        public double Int {
            get => ApplyModificators(b_int, StatModifier.INT);
        }
        public double Agi {
            get => ApplyModificators(b_agi, StatModifier.AGI); 
        }
        public double Str {
            get => ApplyModificators(b_str, StatModifier.STR); 
        }
        public double Kaw {
            get => ApplyModificators(b_kaw, StatModifier.KAW); 
        }
        public double Luck {
            get => ApplyModificators(b_luck, StatModifier.LUCK); 
        }
        public double Psychic {
            get => ApplyModificators(2*Kaw, StatModifier.Psychic); 
        }
        public double Magical {
            get => ApplyModificators(2*Int, StatModifier.Magical); 
        }
        public double Physical {
            get => ApplyModificators(2*Str, StatModifier.Physical); 
        }
        public double CritChance {
            get => ApplyModificators(0.05f + Agi/400, StatModifier.CritChance); 
        }
        public double CritDamage {
            get => ApplyModificators(0.50f + Dex/400, StatModifier.CritDamage); 
        }
        public short stars { get; set; }
        private int XpToLvlUp
        {
            get => (int) Math.Ceiling(50d + 10d*lvl + 0.5d*Math.Pow(lvl, 2));
        }

        public void GiveXP(int _xp)
        {
            xp += _xp;
            if(xp >= XpToLvlUp){
                xp -= XpToLvlUp;
                int temp_xp = xp;
                LevelUp();
                xp = 0;
                GiveXP(temp_xp);
            }
        }
        public void LevelUp()
        {
            lvl++;
            b_str += u_str;
            b_agi += u_agi;
            b_kaw += u_kaw;
            b_int += u_int;
            b_dex += u_dex;
            b_luck += u_luck;
        }
        public void Update(){
            var DBwaifu = Global.waifus[id];
            diffLvlUp = DBwaifu.diffLvlUp;
            imgPATH = DBwaifu.imgPATH;
            o_str = DBwaifu.o_str;
            u_str = DBwaifu.u_str;
            b_str = o_str + (lvl-1)*u_str;
            o_kaw = DBwaifu.o_kaw;
            u_kaw = DBwaifu.u_kaw;
            b_kaw = o_kaw + (lvl-1)*u_kaw;
            o_dex = DBwaifu.o_dex;
            u_dex = DBwaifu.u_dex;
            b_dex = o_dex + (lvl-1)*u_dex;
            o_agi = DBwaifu.o_agi;
            u_agi = DBwaifu.u_agi;
            b_agi = o_agi + (lvl-1)*u_agi;
            o_int = DBwaifu.o_int;
            u_int = DBwaifu.u_int;
            b_int = o_int + (lvl-1)*u_int;
            o_luck = DBwaifu.o_luck;
            u_luck = DBwaifu.u_luck;
            b_luck = o_luck + (lvl-1)*u_luck;
        }

        public Equipment? Equip(Equipment newEquipment)
        {
            // Equips the equipment and check for set changes
            Equipment? oldEquipment = null;
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
            if(equipment.weapon?.setId == equipment.dress?.setId && equipment.dress?.setId == equipment.accessory?.setId && equipment.weapon?.setId != null)
                equipment.set = Global.sets[equipment.weapon.setId];
            else
                equipment.set = null;
                
            return oldEquipment;
        }

        public Equipment? Unequip(EquipmentPiece equipmentPiece)
        {
            Equipment? oldEquipment = null;
            switch(equipmentPiece)
            {
                case EquipmentPiece.Weapon:
                    oldEquipment = equipment.weapon;
                    equipment.weapon = null;
                    break;
                case EquipmentPiece.Dress:
                    oldEquipment = equipment.dress;
                    equipment.dress = null;
                    break;
                case EquipmentPiece.Accessory:
                    oldEquipment = equipment.accessory;
                    equipment.accessory = null;
                    break;
                default:    //??
                    break;
            }
            equipment.set = null;
            return oldEquipment;
        }

        public double GetMultModificators(StatModifier statModifier){
            return new List<Modifier>().Concat(equipment.weapon?.GetAllModifiers() ?? []).Concat(equipment.dress?.GetAllModifiers() ?? []).Concat(equipment.accessory?.GetAllModifiers() ?? []).Concat(equipment.set?.modifiers ?? [])
                .Where(modif => modif?.operationType == OperationType.Multiplicative && modif?.stat == statModifier)
                ?.Aggregate(1.0d, (amount, modificator) => amount += modificator?.amount ?? 0d) ?? 1.0d;
        }

        public double GetAdditiveModificators(StatModifier statModifier){
            return new List<Modifier>().Concat(equipment.weapon?.GetAllModifiers() ?? []).Concat(equipment.dress?.GetAllModifiers() ?? []).Concat(equipment.accessory?.GetAllModifiers() ?? []).Concat(equipment.set?.modifiers ?? [])
                .Where(modif => modif?.operationType == OperationType.Additive && modif?.stat == statModifier)
                ?.Aggregate(0d, (amount, modificator) => amount += modificator?.amount ?? 0d) ?? 0d;
        }

        public double ApplyModificators(double stat, StatModifier statModifier) => 
            (stat + GetAdditiveModificators(statModifier))*GetMultModificators(statModifier);

    }
}
