namespace Nanina.Osu 
{
        public class Beatmapset : ScoreBeatmapset 
        {
                public int bpm {get; set;}
                public bool can_be_hyped{get; set;}
                public required string deleted_at{get; set;}
                public required string last_updated{get; set;}
                public bool discussion_enabled{get; set;}
                public bool discussion_locked{get; set;}
                public bool is_scoreable{get; set;}
                public required string legacy_thread_url{get; set;}
                public required object nominations_summary{get; set;}
                public int ranked{get; set;}
                public required string ranked_date{get; set;}
                public bool storyboard{get; set;}
                public required string submitted_date{get; set;}
                public required string tags{get; set;}
                public required object availability{get; set;}
                public required int[] ratings{get; set;} 
        }

        public class ScoreBeatmapset 
        {
                public required string artist{get; set;}
                public required string artist_unicode{get; set;}
                public required Covers covers {get; set;} 
                public required string creator{get; set;}
                public int favourite_count{get; set;}
                public required string hype{get; set;}
                public double id{get; set;}
                public bool nsfw{get; set;}
                public int offset{get; set;}
                public int play_count{get; set;}
                public required string preview_url{get; set;}
                public required string source{get; set;}
                public bool spotlight{get; set;}
                public required string status{get; set;}
                public required string title{get; set;}
                public required string title_unicode{get; set;}
                public int track_id{get; set;}
                public int user_id{get; set;}
                public bool video{get; set;}
        }

        public class Covers 
        {
                public required string cover {get; set;}
                public required string cover2x {get; set;}
                public required string card {get; set;}
                public required string card2x {get; set;}
                public required string list {get; set;}
                public required string list2x {get; set;}
                public required string slimcover {get; set;}
                public required string slimcover2x {get; set;}
        }
}
