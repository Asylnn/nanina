using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void GetUserFromSessionId(ClientWebSocketResponse rawData){
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            Console.WriteLine("sessionId : " + rawData.data);
            var sessionList = sessionCol.Find(x => x.id == rawData.data);
            if (sessionList.Count() != 1) return;
            var session = sessionList.First();
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user.waifu.ToPoco()));
            if (!session.hasUserAssociatedWithSession) return;
            var userCol = db.GetCollection<User>("userdb");
            var requestedUser = userCol.Find(x => x.Id == session.userId).First();
            Console.WriteLine("username : " + requestedUser.username);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(requestedUser),
            }));

            
        }
    }

    protected void ProvideSessionAndUser(ClientWebSocketResponse rawData)
    {
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            var sessions = sessionCol.Find(x => x.id == rawData.data);
            if(sessions.Count() == 0){
                CreateNewSession();
            }
            else{
                var session = sessions.First();
                Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                    type = "session_id",
                    data = session.id,
                }));
                if(session.hasUserAssociatedWithSession){
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                        type = "user",
                        data = JsonConvert.SerializeObject(DBUtils.GetUser(session.userId)),
                    }));
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = "get banners",
                        data = JsonConvert.SerializeObject(Gacha.banners),
                    }));
                }
            }
        }
    }


    protected SessionDBEntry CreateNewSession()
    {
        var sessionId = Guid.NewGuid().ToString();
        using var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}");
        var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
        Console.WriteLine("Entering new session ID into database! id : " + sessionId);
        var session = new SessionDBEntry {
            userId = null,
            hasUserAssociatedWithSession = false,
            id = sessionId,
            date = Utils.GetTimestamp(),
            lang = "en",
        };
        sessionCol.Insert(session);
        sessionCol.EnsureIndex(x => x.id);
        
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
            type = "session_id",
            data = sessionId,
        }));
        return session;
    }
}