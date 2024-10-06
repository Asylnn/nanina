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
    class Echo : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data);
            Send(e.Data);
        }
        protected override void OnOpen()
        {
            Console.WriteLine("Hello <3");
        }
        /*protected override void OnConnect(MessageEventArgs e)
        {
            Console.WriteLine("Hello <3");
        }*/
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            User user = new User("a nother username");
            Console.WriteLine(user.username + "'s waifu's name is " + user.waifu.name);  
            
            using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
                Console.WriteLine("DB Loaded!"); 
                var user_col = db.GetCollection<PocoUser>("user2");
                user_col.Insert(user.ToPoco());
                user_col.EnsureIndex(x => x.userId);
                var old_users = user_col.Find(x => x.userId == user.userId);
                foreach (PocoUser old_user in old_users)
                {
                    Console.WriteLine("old user name : " + old_user.username);
                }
            }
            var ws = new WebSocketServer("ws://localhost:4889");
            ws.AddWebSocketService<Echo> ("/");
            ws.AddWebSocketService<Echo> ("/test");
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