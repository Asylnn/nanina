/*
Force -> Attaque physique, Minage de minerais
Agilité -> Vitesse d'attaque, Explorations
Intelligence -> Attaque magique, Decryptage
Chance -> (les % des raretés) Loots, Fights
Dextérité -> Coups critiques, Analyse
Kawaii -> Attaque psychique, Maid café
*/
using System.Data;
using Newtonsoft.Json;

public class Waifu
{
    public string name { get; set; }
    public uint xp { get; set; }
    public byte lvl { get; set; }
    public float diffLvlUp { get; set; }
    public string id { get; set; }
    public string imgPATH { get; set; }
    public Equipment weapon { get; set; }
    public Equipment dress { get; set; }
    public Equipment accessory { get; set; }
    public ushort o_str { get; set; }
    public ushort u_str { get; set; }
    public uint b_str { get; set; }
    public ushort o_luck { get; set; }
    public ushort u_luck { get; set; }
    public uint b_luck { get; set; }
    public ushort o_kaw { get; set; }
    public ushort u_kaw { get; set; }
    public uint b_kaw { get; set; }
    public ushort o_int { get; set; }
    public ushort u_int { get; set; }
    public uint b_int { get; set; }
    public uint b_agi { get; set; }
    public ushort o_agi { get; set; }
    public ushort u_agi { get; set; }
    public uint b_dex { get; set; }
    public ushort o_dex { get; set; }
    public ushort u_dex { get; set; }
    public float Dex {
        get => b_dex*GetModificators(12); 
    }
    public float Int {
        get => b_int*GetModificators(8); 
    }
    public float Agi {
        get => b_agi*GetModificators(10); 
    }
    public float Str {
        get => b_str*GetModificators(4); 
    }
    public float Kaw {
        get => b_kaw*GetModificators(6); 
    }
    public float Luck {
        get => b_luck*GetModificators(14); 
    }
    public float Psychic {
        get => 2*Kaw*GetModificators(2); 
    }
    public float Magical {
        get => 2*Int*GetModificators(1); 
    }
    public float Physical {
        get => 2*Str*GetModificators(0); 
    }
    public float CritChance {
        get => (float)(0.05 + Luck/10)*GetModificators(16); 
    }
    public float CritDamage {
        get => (float)(0.50 + Dex/20)*GetModificators(15); 
    }
    public byte stars { get; set; }
    private uint XpToLvlUp
    {
        get => (uint) (diffLvlUp * (10 * lvl + 20));
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

    public void GiveXP(uint _xp)
    {
        xp += _xp;
        if(xp >= XpToLvlUp){
            xp -= XpToLvlUp;
            uint temp_xp = xp;
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
        b_str = (uint)(o_str + (lvl-1)*u_str);
        o_kaw = DBwaifu.o_kaw;
        u_kaw = DBwaifu.u_kaw;
        b_kaw = (uint)(o_kaw + (lvl-1)*u_kaw);
        o_dex = DBwaifu.o_dex;
        u_dex = DBwaifu.u_dex;
        b_dex = (uint)(o_dex + (lvl-1)*u_dex);
        o_agi = DBwaifu.o_agi;
        u_agi = DBwaifu.u_agi;
        b_agi = (uint)(o_agi + (lvl-1)*u_agi);
        o_int = DBwaifu.o_int;
        u_int = DBwaifu.u_int;
        b_int = (uint)(o_int + (lvl-1)*u_int);
        o_luck = DBwaifu.o_luck;
        u_luck = DBwaifu.u_luck;
        b_luck = (uint)(o_luck + (lvl-1)*u_luck);
    }

    public float GetModificators(ushort id){
        var baseAmount = (float) Global.config.additive_or_multiplicative_waifu_modifier[id];
        //The following line probably doesn't work?
        var modificators = weapon?.GetAllModifiers().Concat(dress?.GetAllModifiers()).Concat(accessory?.GetAllModifiers());
        return modificators?.Aggregate(baseAmount, (amount, modif) => amount += modif?.amount ?? 0) ?? baseAmount;
    }
}
