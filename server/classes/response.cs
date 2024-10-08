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
                    using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
                        Console.WriteLine("code : " + rawData.data);
                        Console.WriteLine("url : " + Environment.GetEnvironmentVariable("DISCORD_API_URL") + "token");
                        var base64code = $"{Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")}:{Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")}";
                        Console.WriteLine("unencoded code : " + base64code);
                        Console.WriteLine("encoded code : " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code)));

                        var client = new RestClient(Environment.GetEnvironmentVariable("DISCORD_API_URL"));
                        var request_access_token = new RestRequest("oauth2/token", Method.Post);
                        request_access_token.AddHeader("Authorization", $"Basic {System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code))}");
                        //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request_access_token.AddParameter("grant_type", "authorization_code");
                        request_access_token.AddParameter("code", rawData.data);
                        request_access_token.AddParameter("redirect_uri", Environment.GetEnvironmentVariable("DISCORD_REDIRECT_URI"));
                        //If it doesn't work try "AddQueryParameter"

                        var response_access_token = await client.ExecutePostAsync(request_access_token);
                        var discordTokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordTokenResponse>(response_access_token.Content);
                        //var response = await client.ExecutePostAsync<DiscordTokenResponse>(request);

                        Console.WriteLine("response access token data : " + response_access_token.Content);
                        Console.WriteLine("access token : " + discordTokenResponse.access_token);

                        var request_user_information = new RestRequest("users/@me", Method.Get);
                        request_user_information.AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");
                        var response_user_information = await client.ExecuteGetAsync(request_user_information);
                        Console.WriteLine("response user information data : " + response_user_information.Content);
                        
                        //var discordUserInformationResponse = 

                        /*
                        
                        var request_username = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri(Environment.GetEnvironmentVariable("DISCORD_API_URL") + "@me"),
                        };
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.access_token);
                        var response_username = await Global.client.SendAsync(request);
                        var response_username_string = await response_username.Content.ReadAsStringAsync();
                        Console.WriteLine("DATA : ");
                        Console.WriteLine(response_username_string);
                        var user_col = db.GetCollection<PocoUser>("userdb");
                        User user;

                        var list = user_col.Find(x => x.ids.discordId == rawData.data);
                        if (list.Any()){
                            user = User.FromPoco(list.First());
                        }
                        else {
                            user = new User("", new Ids());
                            //user.ids.discordId = e.Data;
                            //user_col.Insert(user.ToPoco());
                            //user_col.EnsureIndex(x => x.ids.discordId);
                        }
                        
                        Send(Newtonsoft.Json.JsonConvert.SerializeObject(user.ToPoco()));*/
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