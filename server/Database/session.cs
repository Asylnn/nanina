using LiteDB;
using Newtonsoft.Json;

namespace Nanina.Database
{
    public class Session() {
        public string id {get; set;} = Guid.NewGuid().ToString();
        public string? userId {get; set;}
        public ulong date {get; set;} = Utils.GetTimestamp();
        public string locale {get; set;} = Global.config.default_locale;
        public required string webSocketId { get; set; }

        public static Session NewSession(string _webSocketId)
        {
            var session = new Session()
            {
                webSocketId = _webSocketId
            };

            Console.WriteLine("New session with id : " + session.id);
            DBUtils.Insert(session);
            return session;
        }
        
        public void UpdateUserId(string? id) 
        {
            if(id is not null)
            {
                var user = DBUtils.Get<UserData.User>(x => x.Id == id)!;
                user.activeSessionId = this.id;
                DBUtils.Update(user);
            }
            userId = id;
            DBUtils.Update(this);
        }
    }
}
