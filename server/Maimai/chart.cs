namespace Maimai
{
    public class Chart
    {
        public required string chartID;
        public required string difficulty;
        public byte difficultyNum;
        public bool isPrimary;
        public required string level;
        public float levelNum;
        public required string playtype;
        public short songID;
        public required string[] versions;
        public required string[] altTitles;
        public required string artist;
        public short id;
        public required string[] searchTerms;
        public required string title;
        public required Data data;
        public required SongData songData;
    }

    public class Data
    {
        public required string inGameID;
        public required string inGameStrID;
        public float maxPercent;
    }

    public class SongData
    {
        public required string artistJP;
        public required string displayVersion;
        public required string titleJP;
    }
}

