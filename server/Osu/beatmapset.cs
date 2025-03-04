namespace Nanina.Osu 
{
        public class Beatmapset : ScoreBeatmapset 
        {
                public int bpm {get; set;}
                public bool can_be_hyped{get; set;}
                public string deleted_at{get; set;}
                public string last_updated{get; set;}
                public bool discussion_enabled{get; set;}
                public bool discussion_locked{get; set;}
                public bool is_scoreable{get; set;}
                public string legacy_thread_url{get; set;}
                public object nominations_summary{get; set;}
                public int ranked{get; set;}
                public string ranked_date{get; set;}
                public bool storyboard{get; set;}
                public string submitted_date{get; set;}
                public string tags{get; set;}
                public object availability{get; set;}
                public int[] ratings{get; set;} 
        }

        public class ScoreBeatmapset 
        {
                public string artist{get; set;}
                public string artist_unicode{get; set;}
                public Covers covers {get; set;} 
                public string creator{get; set;}
                public int favourite_count{get; set;}
                public string hype{get; set;}
                public double id{get; set;}
                public bool nsfw{get; set;}
                public int offset{get; set;}
                public int play_count{get; set;}
                public string preview_url{get; set;}
                public string source{get; set;}
                public bool spotlight{get; set;}
                public string status{get; set;}
                public string title{get; set;}
                public string title_unicode{get; set;}
                public int track_id{get; set;}
                public int user_id{get; set;}
                public bool video{get; set;}
        }

        public class Covers 
        {
                public string cover {get; set;}
                public string cover2x {get; set;}
                public string card {get; set;}
                public string card2x {get; set;}
                public string list {get; set;}
                public string list2x {get; set;}
                public string slimcover {get; set;}
                public string slimcover2x {get; set;}
        }
}
