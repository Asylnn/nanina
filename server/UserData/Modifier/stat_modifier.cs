namespace Nanina.UserData.ModifierData
{
    public enum StatModifier
    {
        Physical,  //0
        Magical,
        Psychic,
        STR,
        INT,
        KAW,
        AGI,
        DEX,
        LUCK,
        CritDamage,
        CritChance,
        BuffPotency,
        BuffLength,
        DebuffPotency,
        DebuffLength,
        DebuffResist,



        // User Stat Modifier

        EnergyRegen,
        MaxEnergy,
        UnlockFloor,
        UnlockTools,
        
    }
}

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
16 : %CriticalChance
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