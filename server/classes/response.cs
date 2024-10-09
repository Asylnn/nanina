using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using RestSharp;
class Server : WebSocketBehavior
    {
        protected override async void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            switch (rawData.type) {
                case "request user":
                    using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
                        var user_col = db.GetCollection<PocoUser>("userdb");
                        User user = User.FromPoco(user_col.Find(x => x.userId == rawData.data).First());
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user.waifu.ToPoco()));
                        user_col.EnsureIndex(x => x.userId);
                        Send(Newtonsoft.Json.JsonConvert.SerializeObject(user.ToPoco()));
                    }
                    break;
                case "send code":

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
                    var discordTokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordTokenResponse>(response_access_token.Content);

                    Console.WriteLine("response access token data : " + response_access_token.Content);

                    var request_user_information = new RestRequest("users/@me", Method.Get);
                    request_user_information.AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");
                    var response_user_information = await client.ExecuteGetAsync(request_user_information);
                    Console.WriteLine("response user information data : " + response_user_information.Content);
                    var discordUserInformationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordUserInformationResponse>(response_access_token.Content);

                    using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
                        var user_col = db.GetCollection<PocoUser>("userdb");
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
                            user_col.Insert(user.ToPoco());
                            user_col.EnsureIndex(x => x.ids.discordId);
                        }
                        else if(list.Count() == 1){
                            user = User.FromPoco(list.First());
                        }
                        else {
                            user = User.FromPoco(list.First());
                            Console.Error.WriteLine("There is more than two people in the user database with the same discord user id! The id is : " + discordUserInformationResponse.id);
                        }
                        
                        Send(Newtonsoft.Json.JsonConvert.SerializeObject(user.ToPoco()));
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