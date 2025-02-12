using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public void AttributeRandomStatToEquipment(Equipment equipment)
        {
            ModifierWeights[] ModifierWeights = [];
            switch(equipment.piece){
                case EquipmentPiece.Weapon :
                    ModifierWeights = dungeonTemplate.modifierWeightsWeapon;
                    break;
                case EquipmentPiece.Dress :
                    ModifierWeights = dungeonTemplate.modifierWeightsDress;
                    break;
                case EquipmentPiece.Accessory :
                    ModifierWeights = dungeonTemplate.modifierWeightsAccessory;
                    break;
            }
            uint totalWeight = ModifierWeights.Aggregate(0u, (accum, current) => accum + current.weight); //Add all the weights
            Random rng = new ();
            var rand = rng.Next((int)totalWeight);
            for (var i = 0; i <= ModifierWeights.Length; i++){      //Algorithm to get a random id from weights
                
                totalWeight -= ModifierWeights[i].weight;  

                if(totalWeight <= rand)
                {
                    float baseValue = ModifierWeights[i].modifier.operationType == OperationType.Multiplicative ? 
                        Global.baseValues.baseStatsMulti[ModifierWeights[i].modifier.stat.ToString()] : Global.baseValues.baseStatsAdd[ModifierWeights[i].modifier.stat.ToString()]; // <- Cooking
                    var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.baseValues.dungeon_stat_randomness;
                    var amount = baseValue*Global.baseValues.equipment_stat_base_amount_multiplier[dungeonTemplate.difficulty-1];
                    amount = 1 + (amount - 1)*statRandomness;
                    equipment.modifiers.Add( new ()
                    {
                        operationType = ModifierWeights[i].modifier.operationType,
                        stat = ModifierWeights[i].modifier.stat,
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
