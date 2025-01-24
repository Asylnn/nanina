using System.Buffers;
using System.Collections.Generic;

public class PocoWaifu
{
    public string name { get; set; }
    public int xp { get; set; }
    public int lvl { get; set; }
    public float diffLvlUp { get; set; }
    public string id { get; set; }
    public string imgPATH { get; set; }
}

public class Waifu
{
    public string name;
    public string imgPATH; 
    public string id;
    private int xp;
    private int lvl;
    private float diffLvlUp;
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
            imgPATH = imgPATH
        };
    }

    public static Waifu FromPoco(PocoWaifu poco)
    {
        Waifu waifu = new Waifu();
        waifu.name = poco.name;
        waifu.imgPATH = poco.imgPATH;
        waifu.id = poco.id;
        waifu.diffLvlUp = poco.diffLvlUp;
        waifu.lvl = poco.lvl;
        waifu.xp = poco.xp;
        return waifu;
    }
}