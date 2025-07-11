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
        public Modifier stat {get; set;}
        public ushort setId {get; set;}
        public EquipmentPiece piece {get; set;}

        public static IEnumerable<Equipment> CreateEquipmentsForDungeon(ActiveDungeon dungeon, ushort numberOfEquipments)
        {
            var setId = dungeon.template.setRewards.RandomElement();
            var equipments = Global.equipments.FindAll(x => x.setId == setId);
            Console.WriteLine(JsonConvert.SerializeObject(dungeon.template.setRewards));

            for(int i = 0; i < numberOfEquipments; i++)
            {
                var equipment = Utils.DeepCopyReflection(equipments.RandomElement());

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
                equipment.Initialize();
                yield return equipment;
            }
        }

        public void Initialize()
        {
            stat = Utils.DeepCopyReflection(piece switch
            {
                EquipmentPiece.Weapon => Global.baseValues.modifiersWeapon.RandomElement(),
                EquipmentPiece.Dress => Global.baseValues.modifiersDress.RandomElement(),
                EquipmentPiece.Accessory => Global.baseValues.modifiersAccessory.RandomElement(),
                _ => throw new NotImplementedException(),
            });
            Random rng = new ();

            /*uint totalWeight = modifierWeights.Aggregate(0u, (accum, current) => accum + current.weight); //Add all the weights
            var rand = rng.Next((int)totalWeight);
            foreach (var modifierWeight in modifierWeights){      //Algorithm to get a random id from weights
                
                totalWeight -= modifierWeight.weight;  

                if(totalWeight <= rand)
                {
                    float baseValue = modifierWeight.modifier.operationType == OperationType.Multiplicative ?
                            Global.baseValues.baseStatsMulti[modifierWeight.modifier.stat.ToString()]  
                        :   Global.baseValues.baseStatsAdd[modifierWeight.modifier.stat.ToString()];*/
                
            var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.baseValues.dungeon_stat_randomness;
            Console.WriteLine("id " + id);
            Console.WriteLine("rarity " + rarity);
            Console.WriteLine("initial stat " + stat.amount);
            Console.WriteLine("rarity multiplier " + Global.baseValues.equipment_stat_base_amount_multiplier[rarity]);
            Console.WriteLine("randomness " + statRandomness);
            stat.amount *= statRandomness*Global.baseValues.equipment_stat_base_amount_multiplier[rarity];
            Console.WriteLine("final stat " + stat.amount);

            
            /*var amount = baseValue*Global.baseValues.equipment_stat_base_amount_multiplier[rarity]*statRandomness;
            Console.WriteLine(baseValue);
            Console.WriteLine(amount);
            modifiers.Add( new ()
            {
                operationType = modifierWeight.modifier.operationType,
                stat = modifierWeight.modifier.stat,
                amount = amount,
                timeout = 0
            });*/
                
            
        }
    }
}