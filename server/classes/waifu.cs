using System.Buffers;
using System.Collections.Generic;

public class PocoWaifu
{
    public string name { get; set; }
    public int xp { get; set; }
    public int lvl { get; set; }
    public float diffLvlUp { get; set; }
}

public class Waifu
{
    public string name;
    private int xp;
    private int lvl;
    //private string owner;
    private float diffLvlUp;
    private int xpToLvlUp 
    {
        get { return (int) diffLvlUp*(10*lvl + 20); }
        set {}
    }
    public Waifu(string name)
    {
        this.name = name;
        this.lvl = 1;
        this.xp = 0;
        //owner = _owner;
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
            giveXP(temp_xp);
            Console.WriteLine(name + " Just leveled up! She is now level " + (float) lvl);
        }
    }
    public PocoWaifu ToPoco()
    {
        return new PocoWaifu
        {
            name = name,
            xp = xp,
            lvl = lvl,
            diffLvlUp = diffLvlUp
        };
    }

    public static Waifu FromPoco(PocoWaifu poco)
    {
        Waifu waifu = new Waifu(poco.name);
        waifu.diffLvlUp = poco.diffLvlUp;
        waifu.lvl = poco.lvl;
        waifu.xp = poco.xp;
        return waifu;
    }
}