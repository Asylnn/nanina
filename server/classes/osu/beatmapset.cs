//{"cover":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/cover.jpg?1723965442","cover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/cover@2x.jpg?1723965442","card":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/card.jpg?1723965442","card@2x":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/card@2x.jpg?1723965442","list":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/list.jpg?1723965442","list@2x":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/list@2x.jpg?1723965442","slimcover":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/slimcover.jpg?1723965442","slimcover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/2208658\/covers\/slimcover@2x.jpg?1723965442"}
public class OsuBeatmapset : ScoreOsuBeatmapset {
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

public class ScoreOsuBeatmapset {
        public string artist{get; set;}
        public string artist_unicode{get; set;}
        public Object covers{get; set;} //{"cover":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/cover.jpg?1650649403","cover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/cover@2x.jpg?1650649403","card":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/card.jpg?1650649403","card@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/card@2x.jpg?1650649403","list":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/list.jpg?1650649403","list@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/list@2x.jpg?1650649403","slimcover":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/slimcover.jpg?1650649403","slimcover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/slimcover@2x.jpg?1650649403"}
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