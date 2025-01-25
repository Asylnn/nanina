using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void GetSessionFromUserId(ClientWebSocketResponse rawData){
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var sessionCol = db.GetCollection<SessionDBEntry>("sessiondb");
            Console.WriteLine("sessionId : " + rawData.data);
            var sessionList = sessionCol.Find(x => x.sessionId == rawData.data);
            if (sessionList.Count() != 1) return;
            var session = sessionList.First();
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user.waifu.ToPoco()));
            if (!session.hasUserAssociatedWithSession) return;
            var userCol = db.GetCollection<PocoUser>("userdb");
            var requestedUser = userCol.Find(x => x.Id == session.userId).First();
            Console.WriteLine("username : " + requestedUser.username);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(requestedUser) 
            }));
        }
    }

    public void GetSessionId(){ //somehow protected doesn't work?
        Send(JsonConvert.SerializeObject(Communication.UpdateSessionId()));
    }
}