using Nanina.UserData.ModifierData;

namespace Nanina
{
    public class BaseValues {

        public Dictionary<string,float> baseStatsMulti;
        public Dictionary<string,float> baseStatsAdd;
        public float[] equipment_stat_base_amount_multiplier;
        public float dungeon_stat_randomness;
        public uint dungeon_attack_timer_in_milliseconds;
        public ulong time_for_allowing_another_fight_in_milliseconds;
        public ulong time_limit_for_osu_code_verification_in_milliseconds;
        public ulong time_for_allowing_another_claim_in_milliseconds;
        public uint base_gacha_currency_amount;
        public float proportion_of_energy_used_for_each_action;
        public double free_energy_not_used_for_each_action;
        public uint energy_regen_tick_in_seconds;
        public double energy_regen_tick_amount_in_percent;
        public double base_max_energy;
        public string base_theme;
        public double spent_energy_to_gacha_currency_conversion_rate;
        public uint user_xp_for_fights;
        public uint user_xp_for_dungeons;
        public uint maimai_score_expiration_in_milliseconds;
        public List<List<float>> equipment_rarity_probability;
        public double fight_damage_multiplier;
        public List<Modifier> modifiersWeapon;
        public List<Modifier> modifiersDress;
        public List<Modifier> modifiersAccessory;
    }
}
