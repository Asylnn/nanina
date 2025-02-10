using System.Net.Http;
using Newtonsoft.Json;
using RestSharp;

namespace Nanina
{
    public static class Global {
        public static readonly Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../config.json"));
    }

    public class Config {
        public ulong time_for_allowing_another_fight_in_milliseconds;
        public ulong time_limit_for_osu_code_verification_in_milliseconds;
        public ulong time_for_allowing_another_claim_in_milliseconds;
        public bool first_time_running;
        public string default_locale;
        public List<uint> dungeon_accessory_modifier_weights;
        public List<uint> dungeon_dress_modifier_weights;
        public List<uint> dungeon_weapon_modifier_weights;
        public List<ushort> dungeon_accessory_modifier_id;
        public List<ushort> dungeon_dress_modifier_id;
        public List<ushort> dungeon_weapon_modifier_id;
        public float dungeon_stat_randomness;
        public float[] star1_equipment_stat_base_ammount;
        public float star2_equipment_stat_base_ammount_multiplier;
        public float star3_equipment_stat_base_ammount_multiplier;
        public float star4_equipment_stat_base_ammount_multiplier;
        public float star5_equipment_stat_base_ammount_multiplier;
        public uint[] additive_or_multiplicative_waifu_modifier; 
        public uint dungeon_attack_timer_in_milliseconds;
        public string dungeon_storage_path;
        public string database_path;
        public uint automatic_backup_interval_in_seconds;
        public string automatic_backup_database_storage_path;
        public string automatic_backup_database_storage_path2;
        public string osu_tokens_storage_path;
        public string osu_chat_tokens_storage_path;
        public string banners_storage_path;
        public string discord_api_url;
        public string osu_api_url;
        public bool dev;
    }
}
