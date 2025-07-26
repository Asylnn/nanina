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
            if(col.Count() == 0) return default;
            return randomized ? col.Find(func).RandomElement() : col.FindOne(func);
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
                case Beatmap:
                    var mapsCol = (ILiteCollection<Beatmap>) col;
                    mapsCol.EnsureIndex(x => x.id, true);
                    mapsCol.EnsureIndex(x => x.difficulty_rating);
                    mapsCol.EnsureIndex(x => x.nanina_tag);
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
                Type type when type == typeof(Beatmap) => "osumapsdb",
                _ => null
            };
        }

        public static void Update<T>(T thing) {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var collectionName = GetCollectionName<T>();
            var col = db.GetCollection<T>(collectionName);
            col.Update(thing);
        }

        public static void ImportDB(){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            var userdb = JsonConvert.DeserializeObject<UserData.User[]>(File.ReadAllText("../userdb.json"));
            userCol.InsertBulk(userdb);
        }

        public static void ExportDB()
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var userCol = db.GetCollection<UserData.User>("userdb");
            File.WriteAllText("../userdb.json", JsonConvert.SerializeObject(userCol.FindAll()));
        }
    }
}
