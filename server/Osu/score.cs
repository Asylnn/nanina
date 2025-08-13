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
        public long id;
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

        public ScoreDTO ToDTO()
        {
            return new ScoreDTO()
            {
                accuracy = accuracy,
                max_combo = max_combo,
                map_max_combo = beatmap.count_circles + beatmap.count_sliders * 2 + beatmap.count_spinners,
                score = classic_total_score,
                count_miss = statistics.count_miss,
                mods = mods,
                artist = beatmapset.artist,
                creator = beatmapset.creator,
                title = beatmapset.title,
                hit_length = beatmap.hit_length,
                difficulty_rating = beatmap.difficulty_rating,
                version = beatmap.version,
                rank = rank,
            };
        }
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

    public class ScoreDTO
    {
        public float accuracy;
        public int max_combo;
        public int map_max_combo;
        //public int total_score;
        public int score;
        public int count_miss;
        //public required string mode;
        public required Mod[] mods;
        public float difficulty_rating;
        public int hit_length;
        public required string title;
        public required string artist;
        public required string creator;
        public int? timesave;
        public required string version;
        public required string rank;
    }
}

