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
}
