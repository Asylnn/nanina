using System.Xml.XPath;
using Nanina.Database;
using Nanina.Dungeon;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.UserData.ItemData
{
    public enum EquipmentPiece {
        Weapon,
        Dress,
        Accessory,
    }


    
    public class Equipment : Item
    {
        public new ItemType type {get; set;} = ItemType.Equipment;
        public ushort setId {get; set;}
        public EquipmentPiece piece {get; set;}

        public static IEnumerable<Equipment> CreateEquipmentsForDungeon(ActiveDungeon dungeon, ushort numberOfEquipments)
        {
            for(int i = 0; i < numberOfEquipments; i++)
            {
                Console.WriteLine(JsonConvert.SerializeObject(dungeon.template.setRewards));
                var setId = dungeon.template.setRewards.RandomElement();
                var equipments = Global.equipments.FindAll(x => x.setId == setId);
                var equipment = equipments.RandomElement();

                var rarityWeights = Global.baseValues.equipment_rarity_probability[dungeon.template.difficulty-1];
                var totalWeight = rarityWeights.Sum();
                var rand = new Random().NextDouble()*totalWeight;
                //Make switch here
                if(rand < rarityWeights[0])
                    equipment.rarity = 0;
                else if(rand < rarityWeights[0] + rarityWeights[1])
                    equipment.rarity = 1;
                else if(rand < rarityWeights[0] + rarityWeights[1] + rarityWeights[2])
                    equipment.rarity = 2;
                else if(rand < rarityWeights[0] + rarityWeights[1] + rarityWeights[2] + rarityWeights[3])
                    equipment.rarity = 3;
                else
                    equipment.rarity = 4;
                equipment.Initialize(dungeon.template);
                yield return equipment;
            }
        }

        public void Initialize(Dungeon.Template template)
        {
            //Honestly this system should go away... modifiers shouldn't be dependent on dungeon 
            ModifierWeights[] modifierWeights = piece switch
            {
                EquipmentPiece.Weapon => template.modifierWeightsWeapon,
                EquipmentPiece.Dress => template.modifierWeightsDress,
                EquipmentPiece.Accessory => template.modifierWeightsAccessory,
                _ => []
            };

            uint totalWeight = modifierWeights.Aggregate(0u, (accum, current) => accum + current.weight); //Add all the weights
            Random rng = new ();
            var rand = rng.Next((int)totalWeight);
            foreach (var modifierWeight in modifierWeights){      //Algorithm to get a random id from weights
                
                totalWeight -= modifierWeight.weight;  

                if(totalWeight <= rand)
                {
                    float baseValue = modifierWeight.modifier.operationType == OperationType.Multiplicative ?
                            Global.baseValues.baseStatsMulti[modifierWeight.modifier.stat.ToString()]  
                        :   Global.baseValues.baseStatsAdd[modifierWeight.modifier.stat.ToString()];
                    var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.baseValues.dungeon_stat_randomness;
                    Console.WriteLine(id);
                    Console.WriteLine(rarity);
                    Console.WriteLine(Global.baseValues.equipment_stat_base_amount_multiplier[rarity]);
                    Console.WriteLine(statRandomness);
                    
                    Console.WriteLine(baseValue);
                    var amount = baseValue*Global.baseValues.equipment_stat_base_amount_multiplier[rarity]*statRandomness;
                    Console.WriteLine(amount);
                    modifiers.Add( new ()
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
    }
}