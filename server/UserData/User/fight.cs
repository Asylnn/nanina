namespace Nanina.UserData
{
    public class Fight
    {
        public required Game game { get; set; }
        public required string id { get; set; }
        public string? secondaryId { get; set; }
        public ulong timestamp { get; set; } = Utils.GetTimestamp();
        public bool completed { get; set; } = false;
    }
}