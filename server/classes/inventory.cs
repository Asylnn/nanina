/*

WaifuModifiers : 

0 : %PhysicalDmg
1 : %MagicalDmg
2 : %PsychicDmg

3 : +STR
5 : +KAW
7 : +INT
4 : %STR
6 : %KAW
8 : %INT

15 : %CriticalDamage
16 : +CriticalChance
17 : %DebuffPotency
18 : %BuffPotency
19 : %BuffLength
20 : %Debufflength

###########################

9  : +AGI
11 : +DEXT
10 : %AGI
12 : %DEXT

21 : %DebuffPotency
22 : %BuffPotency
23 : +BuffLength
24 : +Debufflength
25 : +DebuffResist

############################

13 : +LUCK
14 : %LUCK

0 : 

*/

public class Inventory {
    public uint equipmentCount;
    public Dictionary<uint, Equipment> equipment {get; set;} = [];
    public Dictionary<uint, Material> material {get; set;} = [];
    public Dictionary<uint, UserConsumable> userConsumable {get; set;} = [];
    public Dictionary<uint, WaifuConsumable> waifuConsumable {get; set;} = [];

    public void AddEquipment(Equipment eqp){
        equipmentCount++;
        eqp.equipmentId = equipmentCount;
        equipment.Add(eqp.equipmentId, eqp);
    }
}

public enum ItemType {
    Equipment,
    UserConsumable,
    WaifuConsumable,
    Material,
}

public enum EquipmentPiece {
    Weapon,
    Dress,
    Accessory,
}

public abstract class Item : ICloneable
{
    public uint count {get; set;}
    public ushort id {get; set;}
    public ItemType type {get; set;}
    public string imgPATH {get; set;}
    public string name {get; set;}
    public string description {get; set;}
    public byte rarity {get; set;}

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class Equipment : Item{
    public new ItemType type {get; set;} = ItemType.Equipment;
    /*public int kaw_flat;
    public int kaw_percent;
    public int int_flat;
    public int int_percent;
    public int agi_flat;
    public int agi_percent;
    public int luck_flat;
    public int luck_percent;
    public int str_flat;
    public int str_percent;
    public int dex_flat;
    public int dex_percent;*/
    public uint equipmentId {get; set;}
    public ushort setId {get; set;}
    public string set_name {get; set;}
    public EquipmentPiece piece {get; set;}
    public WaifuModifier stat {get; set;}
    public WaifuModifier[] modifiers {get; set;}
    public WaifuModifier[] setModifiers {get; set;}

    public WaifuModifier[] GetAllModifiers(){
        return (WaifuModifier[]) modifiers.Concat(setModifiers).Append(stat);
    }
}

public class UserConsumable : Item{
    public new ItemType type = ItemType.UserConsumable;
    public UserModifier[] modifiers {get; set;}
}

public class WaifuConsumable : Item{
    public new ItemType type {get; set;} = ItemType.WaifuConsumable;
    public WaifuConsumable[] modifiers {get; set;}
}

public class Material : Item {
    public new ItemType type {get; set;} = ItemType.Material;
}

public abstract class Modifier {
    public ushort id {get; set;}
    public ulong timeout {get; set;}
    public float amount {get; set;}
}

public class UserModifier : Modifier {}
public class WaifuModifier : Modifier {}