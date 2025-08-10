using Nanina.UserData.ModifierData;
using Newtonsoft.Json;

namespace Nanina
{
    public class BaseValues {

        public required Dictionary<string, float> baseStatsMulti;
        public required Dictionary<string,float> baseStatsAdd;
        public required float[] equipment_stat_base_amount_multiplier;
        public required float dungeon_stat_randomness;
        public required int dungeon_attack_timer_in_milliseconds;
        public required long time_for_allowing_another_fight_in_milliseconds;
        public required long time_limit_for_osu_code_verification_in_milliseconds;
        public required long time_for_allowing_another_claim_in_milliseconds;
        public required int base_gacha_currency_amount;
        public required float proportion_of_energy_used_for_each_action;
        public required double free_energy_not_used_for_each_action;
        public required int energy_regen_tick_in_seconds;
        public required double energy_regen_tick_amount_in_percent;
        public required double base_max_energy;
        public required string base_theme;
        public required double spent_energy_to_gacha_currency_conversion_rate;
        public required int user_xp_for_fights;
        public required int user_xp_for_dungeons;
        public required int maimai_score_expiration_in_milliseconds;
        public required List<List<float>> equipment_rarity_probability;
        public required double fight_damage_multiplier;
        public required List<Modifier> modifiersWeapon;
        public required List<Modifier> modifiersDress;
        public required List<Modifier> modifiersAccessory;
        public required float equipment_main_stat_level_up_multiplicator;
        public required float attribute_stat_randomness;
        public required float cafe_reward_randomness;
        public required int base_activity_length_in_milliseconds;
        public required long continuous_fight_score_expiration_time_in_milliseconds;
    }
}
