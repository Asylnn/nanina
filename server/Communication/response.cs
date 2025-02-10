using WebSocketSharp.Server;
using WebSocketSharp;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        //static private List<WS> webSocketConnections = [];
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
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
                    RequestWaifuDatabase(rawData);
                    break;

                case "update waifu db": 
                    UpdateWaifuDatabase(rawData);
                    break;
                case "request item db" :
                    RequestItemDatabase(rawData);
                    break;

                case "change locale":
                    UpdateLocale(rawData);
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
                    ProvideSessionAndUser(rawData);
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

            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            //webSocketConnections.Remove(this);
            Console.WriteLine("Bye :'(");
        }

        protected override void OnOpen()
        {
            //webSocketConnections.Add(this);
            Console.WriteLine("Hello <3");
        }
    }
}
