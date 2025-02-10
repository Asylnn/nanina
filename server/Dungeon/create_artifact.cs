using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public void AttributeRandomStatToEquipment(Equipment equipment)
        {
            ModifierChance[] modifierChances = [];
            switch(equipment.piece){
                case EquipmentPiece.Weapon :
                    modifierChances = dungeonTemplate.modifierChancesWeapon;
                    break;
                case EquipmentPiece.Dress :
                    modifierChances = dungeonTemplate.modifierChancesDress;
                    break;
                case EquipmentPiece.Accessory :
                    modifierChances = dungeonTemplate.modifierChancesAccessory;
                    break;
            }
            uint start = 0; //0 is an int and not a uint and aggregate want start to be a uint...
            uint totalWeight = modifierChances.Aggregate(start, (accum, current) => accum + current.weight); //Add all the weights
            Random rng = new ();
            var rand = rng.Next((int)totalWeight);  //Ugh, another cast
            for (var i = 0; i <= modifierChances.Length; i++){//Algorithm to get a random id from weights
                
                totalWeight -= modifierChances[i].weight;  

                if(totalWeight <= rand){
                    float baseValue = modifierChances[i].modifier.operationType == OperationType.Multiplicative ? 
                        Global.baseValues.baseStatsMulti[modifierChances[i].modifier.statModifier.ToString()] : Global.baseValues.baseStatsAdd[modifierChances[i].modifier.statModifier.ToString()]; // <- Cooking
                    var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.baseValues.dungeon_stat_randomness;
                    var statDifficultyMultiplier = dungeonTemplate.difficulty switch
                    {
                        1 => 1,
                        2 => Global.baseValues.star2_equipment_stat_base_amount_multiplier,
                        3 => Global.baseValues.star3_equipment_stat_base_amount_multiplier,
                        4 => Global.baseValues.star4_equipment_stat_base_amount_multiplier,
                        5 => Global.baseValues.star5_equipment_stat_base_amount_multiplier,
                        _ => 0,
                    };
                    var amount = baseValue*statDifficultyMultiplier;
                    amount = 1 + (amount - 1)*statRandomness;
                    equipment.modifiers.Add( new ()
                    {
                        operationType = modifierChances[i].modifier.operationType,
                        statModifier = (StatModifier) modifierChances[i].modifier.statModifier,
                        amount = amount,
                        timeout = 0
                    });
                    break;
                }
            }
        }
        public List<Equipment> GetLoot(){
            Random rng = new ();
            
            for(int i = 0; i < dungeonTemplate.numberOfRewards; i++)
            {
                var equipments = DBUtils.GetEquipment(dungeonTemplate.setRewards.ElementAt(rng.Next(dungeonTemplate.setRewards.Length))); //je sais pas ca fait quoi
                Equipment equipment = (Equipment) equipments.ElementAt(rng.Next(equipments.Count)).Clone();

                AttributeRandomStatToEquipment(equipment);
                loot.Add(equipment);
                
            }
            return loot;
        }
    }
}
