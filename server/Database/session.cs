using LiteDB;
using Newtonsoft.Json;

namespace Nanina.Database
{
    public class Session() {
        public string id {get; set;} = Guid.NewGuid().ToString();
        public string userId {get; set;} = null;
        public ulong date {get; set;} = Utils.GetTimestamp();
        public string locale {get; set;} = Global.config.default_locale;
        public string webSocketId {get; set;}
    
        public static Session NewSession(string _webSocketId)
        {
            var session = new Session();
            var sessionCol = DBUtils.GetCollection<Session>();
            session.webSocketId = _webSocketId;
            Console.WriteLine("New session with id : " + session.id);

            sessionCol.Insert(session);
            sessionCol.EnsureIndex(x => x.id, true);
            sessionCol.EnsureIndex(x => x.webSocketId, false);
            return session;
        }
        public void UpdateLocale(string newLocale)
        {
            locale = newLocale;
            if(userId == null){
                var user = DBUtils.GetUser(userId);
                user.locale = locale;
                DBUtils.UpdateUser(user);
            }

            UpdateDB();
        }
        
        public void UpdateDB()
        {
            DBUtils.GetCollection<Session>().Update(this);
        }
        public void UpdateUserId(string id) 
        {
            userId = id;
            UpdateDB();
        }

        public void UpdateWebSocketId(string _webSocketId) 
        {
            webSocketId = _webSocketId;
            UpdateDB();
        }
    }
}
