/*
Force -> Attaque physique, Minage de minerais
Agilité -> Vitesse d'attaque, Explorations
Intelligence -> Attaque magique, Decryptage
Chance -> (les % des raretés) Loots, Fights
Dextérité -> Coups critiques, Analyse
Kawaii -> Attaque psychique, Maid café
*/

using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

public class PocoWaifu
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
}

public class Waifu
{
    public string name;
    public string imgPATH; 
    public string id;
    private int xp;
    private int lvl;
    private float diffLvlUp;
    private int o_str;
    private int u_str;
    private int b_str;
    private int o_luck;
    private int u_luck;
    private int b_luck;
    private int o_kaw;
    private int u_kaw;
    private int b_kaw;
    private int o_int;
    private int u_int;
    private int b_int;
    private int b_agi;
    private int o_agi;
    private int u_agi;
    private int b_dex;
    private int o_dex;
    private int u_dex;
    private int stars;
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

    public void giveXP(int _xp)
    {
        xp += _xp;
        if(xp >= xpToLvlUp){
            xp -= xpToLvlUp;
            int temp_xp = xp;
            lvl++;
            xp = 0;
            Console.WriteLine(name + " Just leveled up! She is now level " + (float) lvl);
            giveXP(temp_xp);
        }
    }
    public PocoWaifu ToPoco()
    {
        return new PocoWaifu
        {
            name = name,
            id = id,
            xp = xp,
            lvl = lvl,
            diffLvlUp = diffLvlUp,
            imgPATH = imgPATH,
            b_str = b_str,
            o_str = o_str,
            u_str = u_str,
            b_agi = b_agi,
            o_agi = o_agi,
            u_agi = u_agi,
            b_luck = b_luck,
            o_luck = o_luck,
            u_luck = u_luck,
            b_int = b_int,
            o_int = o_int,
            u_int = u_int,
            b_kaw = b_kaw,
            o_kaw = o_kaw,
            u_kaw = u_kaw,
            b_dex = b_dex,
            o_dex = o_dex,
        };
    }

    public static Waifu FromPoco(PocoWaifu poco)
    {
        Waifu waifu = new()
        {
            name = poco.name,
            imgPATH = poco.imgPATH,
            id = poco.id,
            diffLvlUp = poco.diffLvlUp,
            lvl = poco.lvl,
            xp = poco.xp,
            b_str = poco.b_str,
            o_str = poco.o_str,
            u_str = poco.u_str,
            b_agi = poco.b_agi,
            o_agi = poco.o_agi,
            u_agi = poco.u_agi,
            b_luck = poco.b_luck,
            o_luck = poco.o_luck,
            u_luck = poco.u_luck,
            b_int = poco.b_int,
            o_int = poco.o_int,
            u_int = poco.u_int,
            b_kaw = poco.b_kaw,
            o_kaw = poco.o_kaw,
            u_kaw = poco.u_kaw,
            b_dex = poco.b_dex,
            o_dex = poco.o_dex,
            u_dex = poco.u_dex,
        };
        return waifu;
    }

    public static void UpdateWaifu(PocoWaifu poco){
        var waifu = DBUtils.GetWaifu(poco.id);
        poco.diffLvlUp = waifu.diffLvlUp;
        poco.imgPATH = waifu.imgPATH;
        poco.name = waifu.name;
    }
}
