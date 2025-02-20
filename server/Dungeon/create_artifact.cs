using Nanina.Database;
using Nanina.Osu;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public void AttributeRandomStatToEquipment(Equipment equipment)
        {
            ModifierWeights[] modifierWeights = equipment.piece switch
            {
                EquipmentPiece.Weapon => dungeonTemplate.modifierWeightsWeapon,
                EquipmentPiece.Dress => dungeonTemplate.modifierWeightsDress,
                EquipmentPiece.Accessory => dungeonTemplate.modifierWeightsAccessory,
                _ => []
            };
            uint totalWeight = modifierWeights.Aggregate(0u, (accum, current) => accum + current.weight); //Add all the weights
            Random rng = new ();
            var rand = rng.Next((int)totalWeight);
            foreach (var modifierWeight in modifierWeights){      //Algorithm to get a random id from weights
                
                totalWeight -= modifierWeight.weight;  

                if(totalWeight <= rand)
                {
                    float baseValue = modifierWeight.modifier.operationType == OperationType.Multiplicative  
                        ?   Global.baseValues.baseStatsMulti[modifierWeight.modifier.stat.ToString()]  
                        :   Global.baseValues.baseStatsAdd[modifierWeight.modifier.stat.ToString()]; // <- Cooking (fair enough!)
                    var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.baseValues.dungeon_stat_randomness;
                    var amount = baseValue*Global.baseValues.equipment_stat_base_amount_multiplier[dungeonTemplate.difficulty-1];
                    amount = 1 + (amount - 1)*statRandomness;
                    equipment.modifiers.Add( new ()
                    {
                        operationType = modifierWeight.modifier.operationType,
                        stat = modifierWeight.modifier.stat,
                        amount = amount,
                        timeout = 0
                    });
                    break;
                }
            }
        }
        public List<Equipment> GetLoot(double spent_energy){
            var numberOfRewards = spent_energy/dungeonTemplate.numberOfRewardsPerEnergy;
            var fraction = numberOfRewards - Math.Floor(numberOfRewards);
            numberOfRewards = Math.Floor(numberOfRewards);
            if(fraction >= new Random().NextDouble())
                numberOfRewards++;
            
            for(int i = 0; i < numberOfRewards; i++)
            {
                var setId = dungeonTemplate.setRewards.RandomElement();
                var equipments = DBUtils.GetEquipmentsFromSet(setId);
                var equipment = equipments.RandomElement();
                

                AttributeRandomStatToEquipment(equipment);
                loot.Add(equipment);
                
            }
            return loot;
        }
    }
}
