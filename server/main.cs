using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Threading;
using LiteDB;
using System.Runtime.Intrinsics.X86;
using WebSocketSharp.Server;
using WebSocketSharp;
using Internal;


namespace Nanina
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Launched!");
            //MyServer.ver();
            LoadServer.Load();
            
            using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
                Console.WriteLine("DB Loaded!"); 
                //var user_col = db.GetCollection<PocoUser>("userdb");
                //var user = new User("Asyln");
                //user.userId = "727";
                //user_col.Insert(user.ToPoco());
                //user_col.EnsureIndex(x => x.userId);
                //user_col.Insert(user.ToPoco());
                //var old_users = user_col.Find(x => x.userId == user.userId);
            }
            Thread.Sleep(3000);
            //OsuApi.GetUserRecentScores("10669137", "osu");
            Console.WriteLine ("Press any key to shut down the server...");
            Console.ReadKey();
        }
    }
}