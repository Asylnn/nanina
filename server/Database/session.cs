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
            var sessionCol = Global.db.GetCollection<Session>("sessiondb");
            session.webSocketId = _webSocketId;
            sessionCol.Insert(session);
            sessionCol.EnsureIndex(x => x.id);
            sessionCol.EnsureIndex(x => x.webSocketId);
            return session;
        }
        public void UpdateLocale(string newLocale){
            
            locale = newLocale;
            
            if(hasUserAssociatedWithSession){
                var user = DBUtils.GetUser(userId);
                user.locale = locale;
                DBUtils.UpdateUser(user);
            }

            UpdateDB();
        }
        
        public void UpdateDB(){
            var sessionCol = Global.db.GetCollection<Session>("sessiondb");
            sessionCol.Update(this);
        }
        public void UpdateUserId(string id) 
        {
            hasUserAssociatedWithSession = true;
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
