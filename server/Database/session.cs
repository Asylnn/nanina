using LiteDB;
using Newtonsoft.Json;

namespace Nanina.Database
{
    public class Session() {
        public string id {get; set;} = Guid.NewGuid().ToString();
        public bool hasUserAssociatedWithSession {get; set;} = false;
        public string userId {get; set;} = null;
        public ulong date {get; set;} = Utils.GetTimestamp();
        public string locale {get; set;} = Global.config.default_locale;
        public string webSocketId {get; set;}
        public bool isConnectedToClient {get; set;} = true;
    
        public static Session NewSession(string _webSocketId)
        {
            var session = new Session();
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<Session>("sessiondb");
            session.webSocketId = _webSocketId;
            Console.WriteLine("New session with id : " + session.id);

            sessionCol.Insert(session);
            sessionCol.EnsureIndex(x => x.id, true);
            sessionCol.EnsureIndex(x => x.webSocketId, false);
            return session;
        }
        public void UpdateLocale(string newLocale){
            
            locale = newLocale;
            Console.WriteLine("hey 1" + newLocale);
            Console.WriteLine(newLocale);
            if(hasUserAssociatedWithSession){
                Console.WriteLine("hey 2");
                var user = DBUtils.GetUser(userId);
                user.locale = locale;
                DBUtils.UpdateUser(user);
            }

            UpdateDB();
        }
        
        public void UpdateDB()
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<Session>("sessiondb");
            sessionCol.Update(this);
            
        }
        public void UpdateUserId(string id) 
        {
            if(id != null)
                hasUserAssociatedWithSession = true;
            else
                hasUserAssociatedWithSession = false;
            userId = id;
            UpdateDB();
        }

        public void UpdateWebSocketId(string _webSocketId, bool _isConnectedToClient) 
        {
            isConnectedToClient = _isConnectedToClient;
            webSocketId = _webSocketId;
            UpdateDB();
        }

    }
}
