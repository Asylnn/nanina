using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
class Server : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
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

                        var user_col = db.GetCollection<PocoUser>("userdb");
                        var user = new User("");

                        var list = user_col.Find(x => x.ids.discordId == rawData.data);
                        if (list.Any()){
                            user = User.FromPoco(list.First());
                        }
                        else {
                            user.ids.discordId = e.Data;
                            user_col.Insert(user.ToPoco());
                            user_col.EnsureIndex(x => x.ids.discordId);
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