using LiteDB;
using Nanina.Communication;
using Nanina.Osu;
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
            var collection = GetCollectionName<T>();
            return db.GetCollection<T>(collection);
        }
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
                case "equipmentdb":
                    prop = "equipment db";
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
            switch(thing){
                case UserData.User:
                    var userCol = (ILiteCollection<UserData.User>) col;
                    userCol.EnsureIndex(x => x.ids.discordId, true);
                    userCol.EnsureIndex(x => x.Id, true);
                    break;
                case Set:
                    var setCol = (ILiteCollection<Set>) col;
                    setCol.EnsureIndex(x => x.id, true);
                    break;
                case Equipment:
                    var equipCol = (ILiteCollection<Equipment>) col;
                    equipCol.EnsureIndex(x => x.id, true);
                    equipCol.EnsureIndex(x => x.type, false);
                    break;
                case Item:
                    var itemCol = (ILiteCollection<Item>) col;
                    itemCol.EnsureIndex(x => x.id, true);
                    itemCol.EnsureIndex(x => x.type, false);
                    break;
                case Beatmap:
                    var mapsCol = (ILiteCollection<Beatmap>) col;
                    mapsCol.EnsureIndex(x => x.id, true);
                    mapsCol.EnsureIndex(x => x.difficulty_rating);
                    break;
                case Waifu:
                    var waifuCol = (ILiteCollection<Waifu>) col;
                    waifuCol.EnsureIndex(x => x.id, true);
                    break;
                case Session:
                    var sessionCol = (ILiteCollection<Session>) col;
                    sessionCol.EnsureIndex(x => x.id, true);
                    sessionCol.EnsureIndex(x => x.webSocketId, false);
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
                Type type when type == typeof(Item) => "itemdb",
                Type type when type == typeof(Set) => "setdb",
                Type type when type == typeof(Equipment) => "equipmentdb",
                _ => null
            };
        }

        public static void Update<T>(T thing) {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            col.Update(thing);
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
                    break;
                case Waifu[]:
                    foreach (var waifu in data){
                        Insert(waifu);
                    }
                    break;
                case Equipment[]:
                    foreach (var equipment in data) {
                        Insert(equipment);
                    }
                    break;
                case Item[]:
                    foreach (var item in data) {
                        Insert(item);
                    }
                    break;
                case null:
                    throw new ArgumentNullException(nameof(data));
                default:
                    break;
            };
        }

        public static List<Equipment> GetEquipmentsFromSet(ushort setId)
        {
            Console.WriteLine(setId);
            var itemCol = GetCollection<Equipment>();
            Console.WriteLine(JsonConvert.SerializeObject(itemCol.FindAll()));
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

        public static void ImportDB(){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            var waifuCol = db.GetCollection<Waifu>("waifudb");
            var itemCol = db.GetCollection<Item>("itemdb");
            var setCol = db.GetCollection<Set>("setdb");
            var osuCol = db.GetCollection<Beatmap>("osumapsdb");
            var equipmentCol = db.GetCollection<Beatmap>("equipmentdb");
            var userdb = JsonConvert.DeserializeObject<UserData.User[]>(File.ReadAllText("../userdb.json"));
            var waifudb = JsonConvert.DeserializeObject<Waifu[]>(File.ReadAllText("../waifudb.json"));
            var itemdb = JsonConvert.DeserializeObject<Item[]>(File.ReadAllText("../itemdb.json"));
            var setdb = JsonConvert.DeserializeObject<Set[]>(File.ReadAllText("../setdb.json"));
            var osudb = JsonConvert.DeserializeObject<Beatmap[]>(File.ReadAllText("../osumapsdb.json"));
            var equipmentdb = JsonConvert.DeserializeObject<Beatmap[]>(File.ReadAllText("../equipmentdb.json"));
            userCol.InsertBulk(userdb);
            waifuCol.InsertBulk(waifudb);
            itemCol.InsertBulk(itemdb);
            setCol.InsertBulk(setdb);
            osuCol.InsertBulk(osudb);
            equipmentCol.InsertBulk(equipmentdb);
        }

        public static void ExportDB()
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            var waifuCol = db.GetCollection<Waifu>("waifudb");
            var itemCol = db.GetCollection<Item>("itemdb");
            var setCol = db.GetCollection<Set>("setdb");
            var osuCol = db.GetCollection<Beatmap>("osumapsdb");
            var equipmentCol = db.GetCollection<Beatmap>("equipmentdb");
            File.WriteAllText("../userdb.json", JsonConvert.SerializeObject(userCol.FindAll()));
            File.WriteAllText("../waifudb.json", JsonConvert.SerializeObject(waifuCol.FindAll()));
            File.WriteAllText("../itemdb.json", JsonConvert.SerializeObject(itemCol.FindAll()));
            File.WriteAllText("../setdb.json", JsonConvert.SerializeObject(setCol.FindAll()));
            File.WriteAllText("../osumapsdb.json", JsonConvert.SerializeObject(osuCol.FindAll()));
            File.WriteAllText("../equipmentdb.json", JsonConvert.SerializeObject(equipmentCol.FindAll()));
        }
    }
}
