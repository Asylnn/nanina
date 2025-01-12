using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;
class WS : WebSocketBehavior
    {
        protected override async void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            switch (rawData.type) {


                //Recieve a request from the client to get a user from a sessionId stored in cookies.
                //It check the database for that sessionId and return, if found, the user associated with that session ID.
                case "request user with session id": 
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
                    break;

                case "update theme": { // bracket are here to have a different scope for the variable named "user".
                    var user = DBUtils.GetUser(rawData.id);
                    user.theme = rawData.data;
                    DBUtils.UpdateUser(user);
                    } break;
                
                case "update osu id": { // bracket are here to have a different scope for the variable named "user".
                    var user = DBUtils.GetUser(rawData.id);
                    user.ids.osuId = rawData.data;
                    DBUtils.UpdateUser(user);
                    } break;

                case "get session id":
                    Send(JsonConvert.SerializeObject(Communication.UpdateSessionId()));
                    break;

                case "connect with discord":
                    Console.WriteLine("code : " + rawData.data);
                    Console.WriteLine("url : " + Environment.GetEnvironmentVariable("DISCORD_API_URL") + "token");
                    var base64code = $"{Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")}:{Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")}";

                    var client = new RestClient(Environment.GetEnvironmentVariable("DISCORD_API_URL"));
                    var request_access_token = new RestRequest("oauth2/token", Method.Post);
                    request_access_token.AddHeader("Authorization", $"Basic {System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code))}");
                    //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request_access_token.AddParameter("grant_type", "authorization_code");
                    request_access_token.AddParameter("code", rawData.data);
                    request_access_token.AddParameter("redirect_uri", Environment.GetEnvironmentVariable("DISCORD_REDIRECT_URI"));

                    var response_access_token = await client.ExecutePostAsync(request_access_token);
                    if(response_access_token.StatusCode != System.Net.HttpStatusCode.OK) {return;}
                    var discordTokenResponse = JsonConvert.DeserializeObject<DiscordTokenResponse>(response_access_token.Content);

                    Console.WriteLine("response access token data : " + response_access_token.Content);

                    var request_user_information = new RestRequest("users/@me", Method.Get);
                    request_user_information.AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");
                    var response_user_information = await client.ExecuteGetAsync(request_user_information);
                    if(response_user_information.StatusCode != System.Net.HttpStatusCode.OK) {return;}

                    Console.WriteLine("response user information data : " + response_user_information.Content);
                    var discordUserInformationResponse = JsonConvert.DeserializeObject<DiscordUserInformationResponse>(response_user_information.Content);

                    using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
                        var user_col = db.GetCollection<PocoUser>("userdb");
                        //We have to unpoco the user for using the constructor
                        User user;

                        var list = user_col.Find(x => x.ids.discordId == discordUserInformationResponse.id);
                        if (list.Count() == 0){
                            var ids = new Ids() {discordId = discordUserInformationResponse.id};
                            var tokens = new Tokens(){
                                discord_access_token = discordTokenResponse.access_token,
                                discord_refresh_token = discordTokenResponse.refresh_token
                            };
                            ids.discordId = discordUserInformationResponse.id;
                            user = new (discordUserInformationResponse.global_name, ids);
                            user.tokens = tokens;
                            user.locale = discordUserInformationResponse.locale;
                            // TODO user.avatarPATH =
                            Console.WriteLine("Inserted new user! Id : " + user.Id);
                            user_col.Insert(user.ToPoco());
                            user_col.EnsureIndex(x => x.ids.discordId, true);
                            user_col.EnsureIndex(x => x.Id, true);
                        }
                        else if(list.Count() == 1){
                            user = User.FromPoco(list.First());
                        }
                        else {
                            user = User.FromPoco(list.First());
                            Console.Error.WriteLine("There is more than two people in the user database with the same discord user id! The id is : " + discordUserInformationResponse.id);
                        }
                        Send(JsonConvert.SerializeObject(Communication.UpdateSessionId(user.Id, true)));
                        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                            type = "user",
                            data = JsonConvert.SerializeObject(user.ToPoco())
                        }));
                    }
                    break;

            }
            //Send(e.Data);
        }
        protected override void OnOpen()
        {
            Console.WriteLine("Hello <3");
        }
    }