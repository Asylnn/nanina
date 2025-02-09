
using System.ComponentModel;
using System.Security.Cryptography;
using Newtonsoft.Json;

public partial class ActiveDungeon {



    public void AttributeRandomStatToEquipment(Equipment equipment){
        List<uint> modifierWeights = [];
        List<ushort> modifierId = [];
        switch(equipment.piece){
            case EquipmentPiece.Weapon :
                modifierWeights = Global.config.dungeon_weapon_modifier_weights;
                modifierId = Global.config.dungeon_weapon_modifier_id;
                break;
            case EquipmentPiece.Dress :
                modifierWeights = Global.config.dungeon_dress_modifier_weights;
                modifierId = Global.config.dungeon_dress_modifier_id;
                break;
            case EquipmentPiece.Accessory :
                modifierWeights = Global.config.dungeon_accessory_modifier_weights;
                modifierId = Global.config.dungeon_accessory_modifier_id;
                break;
        }
        //When applied to a uint List, Sum requires to cast uint into decimal, which is not really problematic but I don't like it.
        //You need to cast here because 0 is an int and not a uint...
        uint totalWeight = (uint) modifierWeights.Aggregate(0, (accum, current) => (int)(accum + current)); //Add all the weights
        Random rng = new ();
        var rand = rng.Next((int)totalWeight);  //Ugh, another cast
        for (var i = 0; i <= modifierWeights.Count; i++){//Algorithm to get a random id from weights
            
            totalWeight -= modifierWeights[i];  

            if(totalWeight <= rand){

                //This only work for multiplicative modifiers 
                var statRandomness = 1 + (float) (rng.NextDouble()*2 - 1)*Global.config.dungeon_stat_randomness;
                var statDifficultyMultiplier = dungeonTemplate.difficulty switch
                {
                    1 => 1,
                    2 => Global.config.star2_equipment_stat_base_ammount_multiplier,
                    3 => Global.config.star3_equipment_stat_base_ammount_multiplier,
                    4 => Global.config.star4_equipment_stat_base_ammount_multiplier,
                    5 => Global.config.star5_equipment_stat_base_ammount_multiplier,
                    _ => 0,
                };
                var amount = Global.config.star1_equipment_stat_base_ammount[modifierId[i]]*statDifficultyMultiplier;
                amount = 1 + (amount - 1)*statRandomness;
                equipment.stat = new WaifuModifier(){
                    id = modifierId[i],
                    amount = amount,
                    timeout = 0
                };
                break;
            }
        }
    }
    public List<Equipment> GetLoot(){
        List<Equipment> loot = new ([]);
        Random rng = new ();
        var setId = dungeonTemplate.setRewards.ElementAt(rng.Next(dungeonTemplate.setRewards.Length));
        var equipments = DBUtils.GetEquipment(setId);

        for(int i = 0; i < dungeonTemplate.numberOfRewards; i++){
            Console.WriteLine(i);

            Console.WriteLine(JsonConvert.SerializeObject(equipments));
            Console.WriteLine(equipments.Count);
            Equipment equipment = (Equipment) equipments.ElementAt(rng.Next(equipments.Count)).Clone();
            Console.WriteLine(JsonConvert.SerializeObject((equipment)));

            AttributeRandomStatToEquipment(equipment);
            loot.Add(equipment);
            
        }
        return loot;
    }
}