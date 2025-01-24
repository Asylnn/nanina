using LiteDB;
public static class DBUtils {
    public static PocoUser GetUser(string id){
        using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
            var userCol = db.GetCollection<PocoUser>("userdb");
            var list = userCol.Find(x => x.Id == id);
            if (list.Count() >= 1){
                return list.First();
            }
            else {
                Console.Error.WriteLine($"Somehow got a userID ({id}) wrong with no associated account? Returning blank user");
                return new PocoUser();
            }
        }
    }

    public static void UpdateUser(PocoUser user){
        using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
            var user_col = db.GetCollection<PocoUser>("userdb");
            user_col.Update(user);
        }
    }
    public static PocoWaifu GetWaifu(string id){
        using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
            var waifuCol = db.GetCollection<PocoWaifu>("waifudb");
            var list = waifuCol.Find(x => x.id == id);
            if (list.Count() >= 1){
                return list.First();
            }
            else {
                Console.Error.WriteLine($"Somehow got a waifu ({id}) wrong with no associated waifu? Returning blank waifu");
                return new PocoWaifu();
            }
        }
    }
}