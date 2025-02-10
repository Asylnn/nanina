using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.Dungeon;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void UpdateLocale(ClientWebSocketResponse rawData){
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            var sessions = sessionCol.Find(x => x.id == rawData.id);
            if(sessions.Count() == 0) {Console.Error.WriteLine("request changing locale without valid session id??"); return;}
            var session = sessions.First();
            session.locale = rawData.data;
            sessionCol.Update(session);
            if(session.hasUserAssociatedWithSession){
                var user = DBUtils.GetUser(session.userId);
                user.locale = rawData.data;
                DBUtils.UpdateUser(user);
            }
        }

        protected void ProvideSessionAndUser(ClientWebSocketResponse rawData)
        {
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
                var sessions = sessionCol.Find(x => x.id == rawData.data);
                if(sessions.Count() == 0){
                    var session = Communication.CreateNewSession();
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                        type = "session",
                        data = JsonConvert.SerializeObject(session),
                    }));
                }
                else{
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
}
