namespace Nanina;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Server Launched!");
        Database.LoadServer.Load();
        Thread.Sleep(1000);
        /*using var db = new LiteDatabase($@"{Global.config.database_path}");
        var col = db.GetCollection<Osu.Beatmap>("osumapsdb");
        col.DeleteAll();*/
        //Maimai.Api.GetRecentScores(Environment.GetEnvironmentVariable("MAITEA_AUTH_KEY"), 592, 11);
        //DBUtils.ImportDB();
        Thread.Sleep(2000);
        Console.WriteLine("Press any key to shut down the server...");
        Console.ReadKey();
    }
}
