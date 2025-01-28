public class Inventory {
    public Dictionary<string, Equipement> equipment;
}

public abstract class Item {
    public int count;
    public int id;
    public string imgPATH;
    public string name;
    public string description;
}

public class Equipement : Item{
    public int kaw_flat;
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
    public int dex_percent;
    public int set_id;
    public string set_name;
    public WaifuModifier[] modifiers;
    public WaifuModifier[] set_modifiers;
}

public class UserConsumable : Item{
    public UserModifier[] modifiers;
}

public class WaifuConsumable : Item{
    public WaifuConsumable[] modifiers;
}

public abstract class Modifier {
    public int id;
    public long timeout;
    public string amount;
}

public class UserModifier : Modifier {}
public class WaifuModifier : Modifier {}