using LiteDB;
using Nanina.UserData;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;

namespace Nanina.Database {
    public static class DBUtils {
        public static User GetUser(string id){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var user_col = db.GetCollection<User>("userdb");
                var list = user_col.Find(x => x.Id == id);
                if (list.Count() >= 1){
                    return list.First();
                }
                else {
                    Console.Error.WriteLine($"Somehow got a userID ({id}) wrong with no associated account? Returning blank user");
                    return new ("name", new());
                }
            }
        }

        public static void UpdateUser(User user){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var user_col = db.GetCollection<User>("userdb");
                user_col.Update(user);
            }
        }
        public static List<Equipment> GetEquipment(ushort setId){
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var itemCol = db.GetCollection<Equipment>("itemdb");
                var items = itemCol.Find(x => x.set.id == setId);
                if (items.Count() >= 1){
                    return items.ToList();
                }
                else {
                    Console.Error.WriteLine($"Somehow got a setId ({setId}) wrong with no associated items? Returning no items");
                    return [];
                }
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
                    Console.Error.WriteLine($"Somehow got a waifu ({id}) wrong with no associated waifu? Returning blank waifu");
                    return new Waifu();
                }
            }
        }
    }
}
