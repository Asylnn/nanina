namespace Nanina.Osu
{
    public class Beatmap : LookUpBeatmap
    {
        /*
            Those properties are Nanina specific and not from osu's api.
        */
        public NaninaStdTag nanina_tag { get; set; }
        //For now let's use our own database for tags, I checked as of 15 march 2025 and you can't get tags from the api.

        public Beatmapset beatmapset { get; set; }
        public object failtimes { get; set; }
    }
    public class LookUpBeatmap
    {
        public int beatmapset_id { get; set; }
        public float difficulty_rating { get; set; }
        public long id { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public int total_length { get; set; }
        public int user_id { get; set; }
        public string version { get; set; }
        public float accuracy { get; set; }
        public float ar { get; set; }
        public float bpm { get; set; }
        public float cs { get; set; }
        public float drain { get; set; }
        public bool convert { get; set; }
        public int count_circles { get; set; }
        public int count_sliders { get; set; }
        public int count_spinners { get; set; }
        public string deleted_at { get; set; }
        public int hit_length { get; set; }
        public bool is_scoreable { get; set; }
        public string last_updated { get; set; }
        public int mode_int { get; set; }
        public int passcount { get; set; }
        public int playcount { get; set; }
        public int ranked { get; set; }
        public string url { get; set; }
        public string checksum { get; set; }
        public int max_combo { get; set; }
    }
}

