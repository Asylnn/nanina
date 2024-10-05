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
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }
    class Program
    {
        
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            

            


            User user = new User("a nother username");
            Console.WriteLine(user.username + "'s waifu's name is " + user.waifu.name);  
            // "Filename=/mnt/storage/storage/Projects/Nanina/save/database.db";
            
            using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
                Console.WriteLine("DB Loaded?");
                
                /*var customer = new Customer
                { 
                    Name = "John Doe", 
                    Phones = new string[] { "8000-0000", "9000-0000" }, 
                    IsActive = true
                };
                var col = db.GetCollection<Customer>("testing");
                col.Insert(customer);
                col.EnsureIndex(x => x.Name);*/
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