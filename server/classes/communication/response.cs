using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;
using System.Security.Claims;

partial class WS : WebSocketBehavior
    {
        protected override async void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            switch (rawData.type) {


                //Recieve a request from the client to get a user from a sessionId stored in cookies.
                //It check the database for that sessionId and return, if found, the user associated with that session ID.
                case "request user with session id": 
                    GetSessionFromUserId(rawData);
                    break;

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

                case "get map to fight": 
                    GetMapToFight(rawData);
                    break;

                 case "claim fight":
                    ClaimFight(rawData);
                    break;

                case "get session id":
                    GetSessionId();
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

            }
        }
        protected override void OnOpen()
        {
            Console.WriteLine("Hello <3");
        }
    }