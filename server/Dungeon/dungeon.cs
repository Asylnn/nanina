/*
Stats offensives pour les waifus :
    KAW = dégats psychiques
    STR = dégats physiques
    INT = dégats magiques
    LUCK = crit chance (200% crit = crit crit / 150% crit = crit + 50% chance crit) /// Tu peux crit tout sauf les débuffs recus
    DEXT = Augmentation durée buff/reduction durée debuffs
    AGI = Crit dmg (+ de dégats mais aussi potency buff/débuff)
*/

using Nanina.Communication;
using Nanina.Database;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public Osu.Beatmap beatmap;
        /*
            The code should allow for any game in the future
        */
        public ulong instanceId;
        public Template template;
        public string userId;
        public string sessionId;
        public List<Waifu> waifus;
        public ulong timestamp = Utils.GetTimestamp();
        public List<DungeonLog> log = [];
        public float health; 
        public float maxHealth;
        public bool isCompleted = false;
        public List<Equipment> loot = [];
        public byte floor;
        public PeriodicTimer damageTimer = new (new (Global.baseValues.dungeon_attack_timer_in_milliseconds*10_000));

        public ActiveDungeon(Template dungeon, UserData.User user, List<Waifu> EquippedWaifus, string _sessionId, ulong _instanceId, byte floor)
        {
            instanceId = _instanceId;
            sessionId = _sessionId;
            template = dungeon;
            template.difficulty = floor;
            template.bossResistances.magicalResistance += 0.05f *(1f - floor);
            template.bossResistances.physicalResistance += 0.05f *(1f - floor);
            template.bossResistances.psychicResistance += 0.05f *(1f - floor);
            userId = user.Id;
            waifus = EquippedWaifus;
            maxHealth = template.maxHealthByFloor[floor-1];
            health = maxHealth;
            StartDungeon();
        }

        public void StopDungeon(){
            damageTimer.Dispose();
            isCompleted = true;
            DungeonManager.UpdateDungeonOfClient(this);
        }
        public async void StartDungeon(){

            beatmap = DBUtils.Get<Beatmap>(x => x.nanina_tag == template.game_playstyle, randomized:true);


            while (await damageTimer.WaitForNextTickAsync())
            {   
                DealDamage();
                if(health <= 0) {
                    damageTimer.Dispose();
                    ConcludeDungeon();
                }
                else 
                    DungeonManager.UpdateDungeonOfClient(this);
            }
        }

        public (float, string) GetDamage(Waifu waifu)
        {
            float dmg;
            string attackType;

            if(waifu.Str >= waifu.Int && waifu.Str >= waifu.Kaw){
                dmg = waifu.Physical*(1 - template.bossResistances.physicalResistance);
                attackType = "physical";
            }
                
            else if(waifu.Int >= waifu.Str && waifu.Int >= waifu.Kaw){
                dmg = waifu.Magical*(1 - template.bossResistances.magicalResistance);
                attackType = "magical";
            }
            else{
                dmg = waifu.Psychic*(1 - template.bossResistances.psychicResistance);
                attackType = "psychic";
            }
            var critDmgMult = (float)(Math.Truncate(waifu.CritChance)*waifu.CritDamage); //Super crit
            var critChance = waifu.CritChance - Math.Truncate(waifu.CritChance);
            var randCrit = new Random().NextDouble();
            
            if(randCrit <= critChance)
                critDmgMult += waifu.CritDamage;
            dmg *= 1 + critDmgMult;
            return (dmg, attackType);
        }
        public void DealDamage(float mult = 1f){
            foreach (var waifu in waifus) 
            {
                
                var (dmg, attackType) = GetDamage(waifu);
                dmg *= mult;

                health -= dmg;
                log.Add(new () {
                    waifuId = waifu.id,
                    waifuName = waifu.name,
                    attackType = attackType,
                    dmg = dmg,
                });
            }
        }

        public override string ToString(){
            return JsonConvert.SerializeObject(this);
        }

        public void ConcludeDungeon(){
            health = 0;
            isCompleted = true;
            List<Loot> lootToServer = [];
            var user = DBUtils.Get<UserData.User>(x => x.Id == userId);
            var (spent_energy, gc) = user.SpendEnergy(1);
            loot = GetLoot(spent_energy);
            foreach(var equipment in loot)
            {
                user.inventory.AddEquipment(equipment);
                lootToServer.Add(new Loot{
                    lootType = LootType.Equipment,
                    item = equipment,
                });
            }
            lootToServer.Add(new Loot{
                lootType = LootType.GC,
                amount = gc,
            });
            user.statCount.total_cleared_dungeon++;
            
            UserData.User.RegenEnergy(user);
            DBUtils.Update(user);
            DungeonManager.UpdateDungeonOfClient(this);
            
            DungeonManager.SendLootToClient(this, lootToServer);
            
        }
    }
}
