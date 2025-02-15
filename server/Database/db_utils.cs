using LiteDB;
using Microsoft.VisualBasic;
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
