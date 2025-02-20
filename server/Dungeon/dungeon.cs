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
using Nanina.UserData;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Dungeon
{
    public partial class ActiveDungeon {
        public ulong instanceId;
        public Template dungeonTemplate;
        public string userId;
        public string sessionId;
        public List<Waifu> waifus;
        public ulong timestamp = Utils.GetTimestamp();
        public List<DungeonLog> log = [];
        public float health; 
        public bool isCompleted = false;
        public List<Equipment> loot = [];
        public PeriodicTimer damageTimer = new (new (Global.baseValues.dungeon_attack_timer_in_milliseconds*10_000));

        public ActiveDungeon(Template dungeon, User user, List<Waifu> EquippedWaifus, string _sessionId, ulong _instanceId)
        {
            instanceId = _instanceId;
            sessionId = _sessionId;
            dungeonTemplate = dungeon;
            userId = user.Id;
            waifus = EquippedWaifus;
            health = dungeonTemplate.maxHealth;
            Console.WriteLine("Dungeon Created!");
            StartDungeon();
        }

        public void StopDungeon(){
            damageTimer.Dispose();
            isCompleted = true;
            DungeonManager.UpdateDungeonOfClient(this);
        }
        public async void StartDungeon(){
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

        public void DealDamage(){
            foreach (var waifu in waifus) 
            {
                float dmg;
                string attackType;

                if(waifu.Str >= waifu.Int && waifu.Str >= waifu.Kaw){
                    dmg = waifu.Physical*(1 - dungeonTemplate.bossResistances.physicalResistance);
                    attackType = "physical";
                }
                    
                else if(waifu.Int >= waifu.Str && waifu.Int >= waifu.Kaw){
                    dmg = waifu.Magical*(1 - dungeonTemplate.bossResistances.magicalResistance);
                    attackType = "magical";
                }
                else{
                    dmg = waifu.Psychic*(1 - dungeonTemplate.bossResistances.psychicResistance);
                    attackType = "psychic";
                }
                var critDmgMult = (float)(Math.Truncate(waifu.CritChance)*waifu.CritDamage); //Super crit
                var critChance = waifu.CritChance - Math.Truncate(waifu.CritChance);
                var randCrit = new Random().NextDouble();
                
                if(randCrit <= critChance)
                    critDmgMult += waifu.CritDamage;
                dmg *= (1 + critDmgMult);
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
            var user = DBUtils.GetUser(userId);
            
            var (spent_energy, gc) = user.SpendEnergy();
            loot = GetLoot(spent_energy);
            foreach(var equipment in loot)
            {
                Console.WriteLine("new loot");
                user.inventory.AddEquipment(equipment);
                lootToServer.Add(new Loot{
                    lootType = LootType.Equipment,
                    item = equipment,
                });
                Console.WriteLine("loot : " + JsonConvert.SerializeObject(lootToServer));
            }
            lootToServer.Add(new Loot{
                lootType = LootType.GC,
                amount = gc,
            });
            user.statCount.total_cleared_dungeon++;
            DBUtils.Update(user);
            User.RegenEnergy(user);
            DungeonManager.UpdateDungeonOfClient(this);
            
            DungeonManager.SendLootToClient(this, lootToServer);
            
        }
    }
}
