namespace Nanina
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Launched!");
            Database.LoadServer.Load();
            Thread.Sleep(2000);
            Console.WriteLine ("Press any key to shut down the server...");
            Console.ReadKey();
        }
    }
}