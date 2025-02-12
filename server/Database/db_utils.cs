using LiteDB;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.Database {
    public static class DBUtils {
        public static UserData.User GetUser(string id){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var user_col = db.GetCollection<UserData.User>("userdb");
                var list = user_col.Find(x => x.Id == id);
                if (list.Count() >= 1){
                    return list.First();
                }
                else {
                    Console.Error.WriteLine($"Somehow got a userID ({id}) wrong with no associated account? Returning null");
                    return null;
                }
            }
        }

        public static void UpdateUser(UserData.User user){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var user_col = db.GetCollection<UserData.User>("userdb");
                user_col.Update(user);
            }
        }
        public static List<Equipment> GetEquipmentFromSet(ushort setId){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var itemCol = db.GetCollection<Equipment>("itemdb");
            var items = itemCol.Find(x => x.setId == setId);
            if (items.Count() >= 1){
                return items.ToList();
            }
            else {
                Console.Error.WriteLine($"Somehow got a setId ({setId}) wrong with no associated items? Returning no items");
                return [];
            }
        
        }
        public static Waifu GetWaifu(string id){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var waifuCol = db.GetCollection<Waifu>("waifudb");
                var list = waifuCol.Find(x => x.id == id);
                if (list.Count() >= 1){
                    return list.First();
                }
                else {
                    Console.Error.WriteLine($"Somehow got a waifu ({id}) wrong with no associated waifu? Returning null");
                    return null;
                }
            }
        }
        public static Session GetSession(string sessionId){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var sessionCol = db.GetCollection<Session>("sessiondb");
                return sessionCol.Find(x => x.id == sessionId).First();
            }
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
