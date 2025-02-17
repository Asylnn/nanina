using WebSocketSharp.Server;
using WebSocketSharp;
using Nanina.Database;
using System.Data.Common;
using LiteDB;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        //static private List<WS> webSocketConnections = [];
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            switch (rawData.type) {

                //Recieve a request from the client to get a user from a sessionId stored in cookies.
                //It check the database for that sessionId and return, if found, the user associated with that session ID.
                case "update theme": 
                    UpdateTheme(rawData);
                    break;
                
                case "update osu id":  
                    UpdateOsuId(rawData);
                    break;

                case "request waifu db" :
                    ProvideWaifuDatabase(rawData);
                    break;

                case "update waifu db": 
                    UpdateWaifuDatabase(rawData);
                    break;
                case "request item db" :
                    ProvideItemDatabase(rawData);
                    break;

                case "change locale":
                    
                    db.GetCollection<Session>("sessiondb").Find(session => session.id == rawData.sessionId).First().UpdateLocale(rawData.data);
                    break;

                case "update item db": 
                    UpdateItemDatabase(rawData);
                    break;

                case "get map to fight": 
                    GetMapToFight(rawData);
                    break;

                 case "claim fight":
                    ClaimFight(rawData);
                    break;

                case "get session id":
                    ProvideSessionToClient(rawData);
                    break;
                    
                case "add beatmap with id": 
                    AddMapToDatabase(rawData);
                    break;

                case "connect with discord":
                    DiscordLogin(rawData);
                    break;
                case "pull request":
                    Pull(rawData);
                    break;
                case "verify osu id":
                    VerifyOsuId(rawData);
                    break;
                case "get map back":
                    SendMapToClient(rawData);
                    break;
                case "start dungeon":
                    StartDungeon(rawData);
                    break;
                case "stop dungeon":
                    StopDungeon(rawData);
                    break;
                case "request set db":
                    ProvideSetDatabase(rawData);
                    break;
                case "update set db":
                    UpdateSetDatabase(rawData);
                    break;
                case "disconect":
                    Disconnect(rawData);
                    break;
                case "equip item":
                    EquipItem(rawData);
                    break;
            }
        }
        protected override void OnClose(CloseEventArgs e)
        {
            var sessionCol = DBUtils.GetCollection<Session>();
            var sessions = sessionCol.Find(session => session.webSocketId == ID);
            if(sessions.Count() >= 1)
            {
                var session = sessions.First();
                session.UpdateWebSocketId(null);
            }
            Console.WriteLine("Bye :'(");
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Hello <3");
        }
    }
}
