namespace Nanina.UserData
{
    public class Fight 
    {
        public string game { get; set; } = null;
        public string id { get; set; } = null;
        public ulong timestamp { get; set; } = 0;
        public bool completed { get; set; } = true;
    }
}