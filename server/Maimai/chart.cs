namespace Maimai
{
    public class Chart
    {
        public string chartID;
        public string difficulty;
        public bool isPrimary;
        public string level;
        public float levelNum;
        public string playtype;
        public short songID;
        public string[] versions;
        public string[] altTitles;
        public string artist;
        public short id;
        public string[] searchTerms;
        public string title;
        public Data data;
    }

    public class Data
    {
        public string inGameID;
        public string inGameStrID;
        public float maxPercent;
    }
}