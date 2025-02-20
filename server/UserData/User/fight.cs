namespace Nanina.UserData
{
    public class Fight 
    {
        public string game { get; set; }
        public string id { get; set; }
        public ulong timestamp { get; set; } = Utils.GetTimestamp();
        public bool completed { get; set; } = false;
    }
}