using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Threading;
using LiteDB;
using System.Runtime.Intrinsics.X86;

namespace Naninaim
{
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
        }
    }
}