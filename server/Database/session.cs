using LiteDB;
using Newtonsoft.Json;

namespace Nanina.Database
{
    public class Session {
        public string id {get; set;} = Guid.NewGuid().ToString();
        public string userId {get; set;} = null;
        public ulong date {get; set;} = Utils.GetTimestamp();
        public string locale {get; set;} = Global.config.default_locale;
        public string webSocketId {get; set;}
    
        public static Session NewSession(string _webSocketId)
        {
            var session = new Session();
            /*get session col to insert newly created session to the db*/
            Console.WriteLine("New session with id : " + session.id);
            DBUtils.Insert(session);
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
            /*get session col to update this session in the db*/
            DBUtils.Update(this);
        }
        public void UpdateUserId(string id) 
        {
            if(id is not null)
            {
                Console.WriteLine("updating user active session Id");
                var user = DBUtils.GetUser(id);
                user.activeSessionId = this.id;
                DBUtils.UpdateUser(user);
            }
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
