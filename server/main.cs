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
            var dotEnvLoadStatus = DotEnv.Load("../.env");
            if (dotEnvLoadStatus == false) {
                System.Environment.Exit(1);
            }
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
            var ws = new WebSocketServer("ws://localhost:4889");
            ws.AddWebSocketService<Server> ("/");
            ws.AddWebSocketService<Server> ("/test");
            ws.Start();
            if (ws.IsListening) {
                Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", ws.Port);

            foreach (var path in ws.WebSocketServices.Paths)
                Console.WriteLine ("- {0}", path);
            }
            Console.WriteLine ("\nPress Enter key to stop the server...");
            Console.ReadLine ();

            ws.Stop ();
        }
    }
}