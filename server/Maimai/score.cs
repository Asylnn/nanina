
namespace Maimai
{
    public class Lang
    {
        public string en;
        public string jp;
    }
    public class Song
    {
        public ushort id;
        public string code;
        public Lang name;
        public Lang artist;
    }
    public class ScoreDetail
    {
        public Stat hits;
        public Stat slide;
        public Stat hold;
        public Stat tap;
        //public Stat break;  <------- !!
    }

    public class Stat
    {
        public ushort perfect;
        public ushort good;
        public ushort great;
        public ushort bad;
    }

    public class DifficultyLevel
    {
        public int key;
        public string value;
        public string label;
    }
    public class Score
    {
        public int id;
        public short achievement;
        public string achievement_formatted;
        public int track;
        public int score;
        public string score_formatted;
        public string rank;
        public byte full_combo;
        //public string full_combo_label; No idea of what this is
        public bool is_high_score;
        public bool is_all_perfect;
        public bool is_track_skip;
        public DifficultyLevel difficulty_level;
        public string play_date;
        public uint play_date_unix;
        public Song song;
        public object player;

    }
}

/*
            "player": {
                "id": 2776,
                "name": "\uff2c\uff55\uff59\uff41\u3000\u03b2",
                "rating": 1634,
                "rating_highest": 1653,
                "level": 1468,
                "play_stats": {
                    "total": 1237,
                    "wins": 1241,
                    "vs": 739,
                    "sync": 1,
                    "first": {
                        "id": 157647,
                        "date": "2023-09-21T15:38:19.000000Z",
                        "date_unix": 1695310699,
                        "api_route": "https:\/\/maitea.app\/api\/v1\/plays\/157647"
                    },
                    "latest": {
                        "id": 649918,
                        "date": "2025-01-09T21:49:02.000000Z",
                        "date_unix": 1736459342,
                        "api_route": "https:\/\/maitea.app\/api\/v1\/plays\/649918"
                    }
                },
                "options": {
                    "icon": {
                        "id": 402,
                        "png": "https:\/\/maitea.app\/storage\/user_icons\/0402.png",
                        "webp": "https:\/\/maitea.app\/storage\/user_icons\/0402.webp"
                    },
                    "icon_deka": null,
                    "nameplate": {
                        "id": 172,
                        "png": "https:\/\/maitea.app\/storage\/user_nameplates\/0172.png",
                        "webp": "https:\/\/maitea.app\/storage\/user_nameplates\/0172.webp"
                    },
                    "frame": {
                        "id": 532,
                        "png": "https:\/\/maitea.app\/storage\/user_frames\/0532.png",
                        "webp": "https:\/\/maitea.app\/storage\/user_frames\/0532.webp"
                    }
                }
            }
        },
        */