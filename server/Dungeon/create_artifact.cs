using Nanina.Database;
using Nanina.Osu;
using Nanina.UserData.ItemData;
using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public List<Equipment> GetLoot(double spent_energy){
            var numberOfRewards = spent_energy/template.numberOfRewardsPerEnergy;
            var fraction = numberOfRewards - Math.Floor(numberOfRewards);
            numberOfRewards = Math.Floor(numberOfRewards);
            if(fraction >= new Random().NextDouble())
                numberOfRewards++;

            loot.AddRange(Equipment.CreateEquipmentsForDungeon(this, (ushort) numberOfRewards));
            return loot;
        }
    }
}
