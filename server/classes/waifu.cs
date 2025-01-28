/*
Force -> Attaque physique, Minage de minerais
Agilité -> Vitesse d'attaque, Explorations
Intelligence -> Attaque magique, Decryptage
Chance -> (les % des raretés) Loots, Fights
Dextérité -> Coups critiques, Analyse
Kawaii -> Attaque psychique, Maid café
*/
public class Waifu
{
    public string name { get; set; }
    public int xp { get; set; }
    public int lvl { get; set; }
    public float diffLvlUp { get; set; }
    public string id { get; set; }
    public string imgPATH { get; set; }
    public int o_str { get; set; }
    public int u_str { get; set; }
    public int b_str { get; set; }
    public int o_luck { get; set; }
    public int u_luck { get; set; }
    public int b_luck { get; set; }
    public int o_kaw { get; set; }
    public int u_kaw { get; set; }
    public int b_kaw { get; set; }
    public int o_int { get; set; }
    public int u_int { get; set; }
    public int b_int { get; set; }
    public int b_agi { get; set; }
    public int o_agi { get; set; }
    public int u_agi { get; set; }
    public int b_dex { get; set; }
    public int o_dex { get; set; }
    public int u_dex { get; set; }
    public short stars { get; set; }
    private int xpToLvlUp 
    {
        get { return (int) diffLvlUp*(10*lvl + 20); }
        set {}
    }
    public Waifu()
    {
        this.name = "no_name";
        this.id = "-1";
        this.lvl = 1;
        this.xp = 0;
        this.imgPATH = "no_waifu_img";
        diffLvlUp = 1;
    }

    public void GiveXP(int _xp)
    {
        xp += _xp;
        if(xp >= xpToLvlUp){
            xp -= xpToLvlUp;
            int temp_xp = xp;
            lvl++;
            xp = 0;
            Console.WriteLine(name + " Just leveled up! She is now level " + (float) lvl);
            GiveXP(temp_xp);
        }
    }
    public static void UpdateWaifu(Waifu poco){
        var waifu = DBUtils.GetWaifu(poco.id);
        poco.diffLvlUp = waifu.diffLvlUp;
        poco.imgPATH = waifu.imgPATH;
        poco.name = waifu.name;
    }
}
