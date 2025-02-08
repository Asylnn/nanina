public class Inventory {
    public Dictionary<string, Equipment> equipment;
    public Dictionary<string, Material> material;
    public Dictionary<string, UserConsumable> userConsumable;
    public Dictionary<string, WaifuConsumable> waifuConsumable;
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

public abstract class Item {
    public int count {get; set;}
    public int id {get; set;}
    public ItemType type {get; set;}
    public string imgPATH {get; set;}
    public string name {get; set;}
    public string description {get; set;}
    public short rarity {get; set;}
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
    public int setId {get; set;}
    public string set_name {get; set;}
    public EquipmentPiece piece {get; set;}
    public WaifuModifier[] modifiers {get; set;}
    public WaifuModifier[] setModifiers {get; set;}
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
    public new ItemType type {get; set;}= ItemType.Material;
}

public abstract class Modifier {
    public int id {get; set;}
    public long timeout {get; set;}
    public float amount {get; set;}
}

public class UserModifier : Modifier {}
public class WaifuModifier : Modifier {}