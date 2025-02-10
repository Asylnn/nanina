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
16 : +CriticalChance
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
using Nanina.UserData.ItemData;

namespace Nanina.UserData
{
    public class Inventory 
    {
        public uint equipmentCount {get; set;} = 0;
        public Dictionary<uint, Equipment> equipment {get; set;} = [];
        public Dictionary<uint, Material> material {get; set;} = [];
        public Dictionary<uint, UserConsumable> userConsumable {get; set;} = [];
        public Dictionary<uint, WaifuConsumable> waifuConsumable {get; set;} = [];

        public void AddEquipment(Equipment eqp){
            equipmentCount++;
            eqp.equipmentId = equipmentCount;
            equipment.Add(eqp.equipmentId, eqp);
        }
    }
}










