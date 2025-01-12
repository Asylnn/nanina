using LiteDB;
public static class DBUtils {
    public static PocoUser GetUser(string id){
        using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
            var user_col = db.GetCollection<PocoUser>("userdb");
            //We have to unpoco the user for using the constructor
            var list = user_col.Find(x => x.Id == id);
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
}