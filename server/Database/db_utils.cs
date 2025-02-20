using System.Reflection;
using LiteDB;
using Microsoft.VisualBasic;
using Nanina.Communication;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.Database 
{
    public static class DBUtils {

        /*
            This static class is for managing the database
        */
        public static ILiteCollection<T> GetCollection<T>()
        {
            var db = new LiteDatabase($@"{Global.config.database_path}");
            var collection = typeof(T) switch {
                Type type when type == typeof(UserData.User) => "userdb",
                Type type when type == typeof(Session) => "sessiondb",
                Type type when type == typeof(Waifu) => "waifudb",
                Type type when type == typeof(Beatmap) => "osumapsdb",
                Type type when type == typeof(Item) || type == typeof(Equipment) => "itemdb",
                Type type when type == typeof(Set) => "setdb",
                _ => null
            };
            return db.GetCollection<T>(collection);
        }

        /*Different use cases :
        - access to a col to find one #Done
        - access to a col to insert one (no test) #Done
        - access to a col to insert one (after testing for duplicate) #Done
        - access to a col to update one #Done
        - access to a col to delete all into insert all back #Done
        - access to a col to pick a random one #Done
        - access to a col to find all and send the whole table via a websocket #Done
        - access to a col to find all (without websocket) #Done
        */
        public static T Get<T>(System.Linq.Expressions.Expression<Func<T, bool>> func, bool randomized = false)
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            return randomized ? col.Find(func).RandomElement() : col.FindOne(func);
        }

        public static void SendDatabaseToClient<T>(string webSocketId)
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            var theEnum = col.FindAll();
            var prop = "";
            switch (collectionName){
                case "waifudb":
                    prop = "waifu db";
                    break;
                case "setdb":
                    prop = "set db";
                    break;
                case "itemdb":
                    prop = "item db";
                    break;
            }
            if(prop != "") {
                var data = JsonConvert.SerializeObject(theEnum);
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = prop,
                        data = data
                    }), webSocketId);
            }
        }

        public static void Insert<T>(T thing)
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            col.Insert(thing);
            Console.WriteLine("insert done");
            switch(thing){
                case UserData.User:
                    var userCol = (ILiteCollection<UserData.User>) col;
                    userCol.EnsureIndex(x => x.ids.discordId, true);
                    userCol.EnsureIndex(x => x.Id, true);
                    Console.WriteLine("ensure index done");
                    break;
                case Set:
                    var setCol = (ILiteCollection<Set>) col;
                    setCol.EnsureIndex(x => x.id, true);
                    Console.WriteLine("ensure index done");
                    break;
                case Equipment:
                    var equipCol = (ILiteCollection<Equipment>) col;
                    equipCol.EnsureIndex(x => x.setId, false);
                    var eItemCol = (ILiteCollection<Item>) col;
                    eItemCol.EnsureIndex(x => x.id, true);
                    eItemCol.EnsureIndex(x => x.type, false);
                    Console.WriteLine("ensure index done");
                    break;
                case Item:
                    var itemCol = (ILiteCollection<Item>) col;
                    itemCol.EnsureIndex(x => x.id, true);
                    itemCol.EnsureIndex(x => x.type, false);
                    Console.WriteLine("ensure index done");
                    break;
                case Beatmap:
                    var mapsCol = (ILiteCollection<Beatmap>) col;
                    mapsCol.EnsureIndex(x => x.id, true);
                    mapsCol.EnsureIndex(x => x.difficulty_rating);
                    Console.WriteLine("ensure index done");
                    break;
                case Waifu:
                    var waifuCol = (ILiteCollection<Waifu>) col;
                    waifuCol.EnsureIndex(x => x.id, true);
                    Console.WriteLine("ensure index done");
                    break;
                case Session:
                    var sessionCol = (ILiteCollection<Session>) col;
                    sessionCol.EnsureIndex(x => x.id, true);
                    sessionCol.EnsureIndex(x => x.webSocketId, false);
                    Console.WriteLine("ensure index done");
                    break;
                case null:
                    throw new ArgumentNullException(nameof(thing));
                default:
                    break;
            };
        }

        public static bool isMapADuplicate(Beatmap map){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<Beatmap>();
            var col = db.GetCollection<Beatmap>(collectionName);
            return col.Exists(x => x.id == map.id);
            
        }

        private static string GetCollectionName<T>()
        {
            return typeof(T) switch {
                Type type when type == typeof(UserData.User) => "userdb",
                Type type when type == typeof(Session) => "sessiondb",
                Type type when type == typeof(Waifu) => "waifudb",
                Type type when type == typeof(Beatmap) => "osumapsdb",
                Type type when type == typeof(Item) || type == typeof(Equipment) => "itemdb",
                Type type when type == typeof(Set) => "setdb",
                _ => null
            };
        }

        public static void Update<T>(T thing) {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            col.Update(thing);
            Console.WriteLine("insert done");

        }

        public static void Rebuild<T>(T[] data) {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            col.DeleteAll();
            switch(data){
                case Set[] :
                    foreach (var set in data) {
                        Insert(set);
                    }
                    Console.WriteLine("Rebuilt Set db");
                    break;
                case Waifu[]:
                    Console.WriteLine("Entered rebuild waifu db");
                    foreach (var waifu in data){
                        Insert(waifu);
                    }
                    Console.WriteLine("Rebuilt Waifu db");
                    break;
                case Equipment[]:
                    //Later
                    break;
                case Item[]:
                    //Later
                    break;
                case null:
                    throw new ArgumentNullException(nameof(data));
                default:
                    break;
            };
        }


        public static UserData.User GetUser(string id)
        {
            var userCol = GetCollection<UserData.User>();
            var user = userCol.FindOne(x => x.Id == id);
            if (user != null){
                return user;
            }
            else {
                Console.Error.WriteLine($"Somehow got a userID ({id}) wrong with no associated account? Returning null");
                return null;
            }
        }

        public static void UpdateUser(UserData.User user){
            GetCollection<UserData.User>().Update(user);
        }
        public static List<Equipment> GetEquipmentsFromSet(ushort setId)
        {
            var itemCol = GetCollection<Equipment>();
            var items = itemCol.Find(x => x.setId == setId);
            if (items.Count() >= 1)
            {
                return items.ToList();
            }
            else 
            {
                Console.Error.WriteLine($"Somehow got a setId ({setId}) wrong with no associated items? Returning no items");
                return [];
            }
        
        }
        public static Waifu GetWaifu(string id)
        {
            var waifuCol = GetCollection<Waifu>();
            var waifu = waifuCol.FindOne(x => x.id == id);
            if (waifu != null){
                return waifu;
            }
            else {
                Console.Error.WriteLine($"Somehow got a waifu ({id}) wrong with no associated waifu? Returning null");
                return null;
                
            }
        }
        public static Session GetSession(string sessionId)
        {
            return GetCollection<Session>().FindOne(x => x.id == sessionId);
        }

        public static Beatmap GetMap(string id)
        {
            return GetCollection<Beatmap>().FindOne(x => x.id == Convert.ToInt64(id));
        }

        public static void ImportDB(){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            var waifuCol = db.GetCollection<Waifu>("waifudb");
            var itemCol = db.GetCollection<Item>("itemdb");
            var setCol = db.GetCollection<Set>("setdb");
            var osuCol = db.GetCollection<Beatmap>("osumapsdb");
            var userdb = JsonConvert.DeserializeObject<UserData.User[]>(File.ReadAllText("../userdb.json"));
            var waifudb = JsonConvert.DeserializeObject<Waifu[]>(File.ReadAllText("../waifudb.json"));
            var itemdb = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("../itemdb.json"));
            var setdb = JsonConvert.DeserializeObject<Set[]>(File.ReadAllText("../setdb.json"));
            var osudb = JsonConvert.DeserializeObject<Beatmap[]>(File.ReadAllText("../osumapsdb.json"));
            userCol.InsertBulk(userdb);
            waifuCol.InsertBulk(waifudb);
            itemCol.InsertBulk(itemdb);
            setCol.InsertBulk(setdb);
            osuCol.InsertBulk(osudb);
        }

        public static void ExportDB(){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            var waifuCol = db.GetCollection<Waifu>("waifudb");
            var itemCol = db.GetCollection<Item>("itemdb");
            var setCol = db.GetCollection<Set>("setdb");
            var osuCol = db.GetCollection<Beatmap>("osumapsdb");
            File.WriteAllText("../userdb.json", JsonConvert.SerializeObject(userCol.FindAll()));
            File.WriteAllText("../waifudb.json", JsonConvert.SerializeObject(waifuCol.FindAll()));
            File.WriteAllText("../itemdb.json", JsonConvert.SerializeObject(itemCol.FindAll()));
            File.WriteAllText("../setdb.json", JsonConvert.SerializeObject(setCol.FindAll()));
            File.WriteAllText("../osumapsdb.json", JsonConvert.SerializeObject(osuCol.FindAll()));
        }
    }
}
