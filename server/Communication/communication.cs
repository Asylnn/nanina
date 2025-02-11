using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.Dungeon;
using Nanina.UserData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        

        protected void ProvideSessionAndUser(ClientWebSocketResponse rawData)
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<Session>("sessiondb");
            var sessions = sessionCol.Find(x => x.id == rawData.data);
            if(sessions.Count() == 0)
            {
                var session = Session.NewSession(this.ID);
                Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                    type = "session",
                    data = JsonConvert.SerializeObject(session),
                }));
            }
            else
            {
                var session = sessions.First();
                Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                    type = "session",
                    data = JsonConvert.SerializeObject(session),
                }));
                if(session.hasUserAssociatedWithSession){
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                        type = "user",
                        data = JsonConvert.SerializeObject(DBUtils.GetUser(session.userId)),
                    }));
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = "get banners",
                        data = JsonConvert.SerializeObject(GachaManager.banners),
                    }));
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = "get dungeons",
                        data = JsonConvert.SerializeObject(DungeonManager.dungeons),
                    }));
                }
            }
            
        }
    }
}
