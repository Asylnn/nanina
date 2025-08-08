namespace Nanina.Osu
{
    public class ScoreStatistics 
    {
        public int count_300;
        public int count_100;
        public int count_50;
        public int count_geki;
        public int count_katu;
        public int count_miss;
    }
    public class Score 
    {
        public float accuracy;
        public double beatmap_id;
        public double best_id;
        public double build_id;
        public int classic_total_score;
        public required string ended_at;
        public bool has_replay;
        public ulong id;
        public bool is_perfect_combo;
        public int legacy_perfect;
        public int legacy_score_id;
        public int legacy_total_score;
        public int max_combo;
        public required ScoreStatistics maximum_statistics; 	
        public required Mod[] mods;
        public bool passed;
        public int playlist_item_id;
        public float pp;
        public bool preserve;
        public bool processed;
        public required string rank;
        public bool ranked;
        public int room_id;
        public int ruleset_id;
        public required string started_at;
        public required ScoreStatistics statistics;
        public int total_score;
        public required string type;
        public double user_id;
        public required User user;
        public required LookUpBeatmap beatmap;
        public required ScoreBeatmapset beatmapset;
    }

    public class ScoreExtended : Score
    {
        public required string created_at;
        public required string mode;
        public int mode_int;
        public bool perfect;
        public bool replay;
        public int score;
        public required object current_user_attributes; // "current_user_attributes":{"pin":null
    }
}

