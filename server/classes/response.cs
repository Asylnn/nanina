using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
class Server : WebSocketBehavior
    {
        protected override async void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            switch (rawData.type) {
                case "request user":
                    using(var db = new LiteDatabase(@"/mnt/storage/storage/Projects/Nanina/save/database.db")){
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
                        Console.WriteLine("url : " + base64code);

                        var request = new HttpRequestMessage
                        {
                            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new 
                                {
                                    grant_type = "authorization_code",
                                    code = rawData.data,
                                    redirect_uri = Environment.GetEnvironmentVariable("DISCORD_REDIRECT_URI"),
                                }
                            )),
                            Method = HttpMethod.Post,
                            RequestUri = new Uri(Environment.GetEnvironmentVariable("DISCORD_API_URL") + "token"),
                        };
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code)));
                        //request.Headers.Accept.Add("Content-Type", "application/x-www-form-urlencoded");
                        //request.Headers.Add("Authorization", $"Basic {System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code))}");
                        var response = await Global.client.SendAsync(request);
                        //response.EnsureSuccessStatusCode();
                        var string_content = await response.Content.ReadAsStringAsync();
                        var tokens = Newtonsoft.Json.JsonConvert.DeserializeObject<DiscordTokenResponse>(string_content);
                        Console.WriteLine("DISCORD TOKENS : ");
                        Console.WriteLine(string_content);
                        Console.WriteLine(tokens);
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