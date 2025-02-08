/*
Stats offensives pour les waifus :
    KAW = dégats psychiques
    STR = dégats physiques
    INT = dégats magiques
    LUCK = crit chance (200% crit = crit crit / 150% crit = crit + 50% chance crit) /// Tu peux crit tout sauf les débuffs recus
    DEXT = Augmentation durée buff/reduction durée debuffs
    AGI = Crit dmg (+ de dégats mais aussi potency buff/débuff)
    </li>
*/


using System.Reflection.Metadata.Ecma335;

public class Dungeon {
    public string id;
    public short numberOfRewards;
    public float[] bossDmgResist;
    public string[] setRewards; 
    public float maxHealth;
}

public class DungeonLog {
    public string waifuId;
    public string attackType;
    public float dmg;
}

public partial class ActiveDungeon : Dungeon {
    public string id;
    public User user = null;
    public float[] bossDmgResist;
    public List<Waifu> waifus;
    public long timestamp;
    public List<DungeonLog> log;
    public float health;

    public async void StartDungeon(){
        health = maxHealth;
    }
    public void DealDamage(){
        waifus.ForEach(waifu => {
            float dmg;
            string attackType;
            if(waifu._str >= waifu._int && waifu._str >= waifu._kaw){
                dmg = waifu.physical;
                attackType = "physical";
            }
                
            else if(waifu._int >= waifu._str && waifu._int >= waifu._kaw){
                dmg = waifu.magical;
                attackType = "magical";
            }
            else{
                attackType = "magical";
                dmg = waifu.psychic;
            }
            health -= dmg;
            log.Add(new () {
                waifuId = waifu.id,
                attackType = attackType,
                dmg = dmg,
            });
            if(health <= 0){
                ConcludeDungeon();
            }
        });
    }

    public void ConcludeDungeon(){
        Random rng = new ();
        var random_elem = rng.Next(setRewards.Count());
        var map = setRewards.ElementAt(random_elem);
    }
}