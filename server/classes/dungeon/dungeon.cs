/*
Stats offensives pour les waifus :
    KAW = dégats psychiques
    STR = dégats physiques
    INT = dégats magiques
    LUCK = crit chance (200% crit = crit crit / 150% crit = crit + 50% chance crit) /// Tu peux crit tout sauf les débuffs recus
    DEXT = Augmentation durée buff/reduction durée debuffs
    AGI = Crit dmg (+ de dégats mais aussi potency buff/débuff)
*/


using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using WebSocketSharp.Server;

public class BossResistances {
    public float physicalResistance;
    public float magicalResistance;
    public float psychicResistance;
} 
public class DungeonTemplate {
    public string id;
    public short numberOfRewards;
    public BossResistances bossResistances;
    public ushort[] setRewards; 
    public float maxHealth;
    public byte difficulty;
}

public class DungeonLog {
    public string waifuId;
    public string attackType;
    public float dmg;
}

public partial class ActiveDungeon {
    public ulong instanceId;
    public WebSocketSessionManager WSSession;
    public string webSocketId;
    public DungeonTemplate dungeonTemplate;
    public string userId;
    public List<Waifu> waifus;
    public ulong timestamp = Utils.GetTimestamp();
    public List<DungeonLog> log = [];
    public float health; 
    public bool isCompleted = false;
    public List<Equipment> loot;
    public PeriodicTimer damageTimer = new (new (Global.config.dungeon_attack_timer_in_milliseconds*10_000));

    public ActiveDungeon(DungeonTemplate dungeon, User user, List<Waifu> EquipedWaifus, string wsId, WebSocketSessionManager session, ulong _instanceId){
        instanceId = _instanceId;
        WSSession = session;
        webSocketId = wsId;
        dungeonTemplate = dungeon;
        userId = user.Id;
        waifus = [EquipedWaifus.First()];
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
            Console.WriteLine("Dungeon Started!");
            DealDamage();
            
             
            if(health <= 0) {
                health = 0;
                isCompleted = true;
                
                
                ConcludeDungeon();
                DungeonManager.UpdateDungeonOfClient(this); //I don't know if Disposing the timer cut the execution, so i'm putting this here for now
                WSSession = null;
                damageTimer.Dispose();
            }
            else {
                DungeonManager.UpdateDungeonOfClient(this); //I don't like this tbh...
            }
           
                
        }
    }

    public void DealDamage(){
        foreach (var waifu in waifus) {
            Random rng = new();
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
                attackType = "psychic";
                dmg = waifu.Psychic*(1 - dungeonTemplate.bossResistances.psychicResistance);
            }
            var critDmgMult = (float)(Math.Truncate(waifu.CritChance)*waifu.CritDamage); //Super crit
            var randCrit = rng.NextDouble();
            var critChance = waifu.CritChance - Math.Truncate(waifu.CritChance);
            if(randCrit <= critChance)
                critDmgMult += waifu.CritDamage;
            dmg *= (1 + critDmgMult);
            health -= dmg;
            log.Add(new () {
                waifuId = waifu.id,
                attackType = attackType,
                dmg = dmg,
            });
            //DungeonManager.UpdateDungeonOfClient(this);
        }
    }

    public ActiveDungeon ToClient(){
        ActiveDungeon dungeon = (ActiveDungeon)MemberwiseClone();
        dungeon.WSSession = null;
        dungeon.webSocketId = null;
        return dungeon;
    }

    public override string ToString(){
        return JsonConvert.SerializeObject(ToClient());
    }

    public void ConcludeDungeon(){
        loot = GetLoot();
        var user = DBUtils.GetUser(userId);
        loot.ForEach(equipment => user.inventory.AddEquipment(equipment));
        DBUtils.UpdateUser(user);
    }
}