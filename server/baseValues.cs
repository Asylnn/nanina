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
    }
}
