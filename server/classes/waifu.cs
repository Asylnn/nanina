using System.Buffers;
using System.Collections.Generic;

class PocoWaifu
{
    public string name { get; set; }
    public int xp { get; set; }
    public int lvl { get; set; }
}

class Waifu
{
    public string name;
    private int xp;
    private int lvl;
    private string owner;
    private float diffLvlUp;
    private int xpToLvlUp 
    {
        get { return (int) diffLvlUp*(10*lvl + 20); }
        set {}
    }
    public Waifu(string _name, string _owner)
    {
        name = _name;
        owner = _owner;
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
            lvl = lvl
        };
    }
}