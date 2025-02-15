using WebSocketSharp.Server;
using LiteDB;
using Newtonsoft.Json;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.Communication;
using Nanina.UserData.WaifuData;
using System.Net;

namespace Nanina.Database
{
    public static class LoadServer {

        public static async void Load(){
            //LoadConfig();
            LoadEnv();
            LoadOsuApi();
            LoadWebSocketServer();
            if(Global.config.first_time_running) 
                FirstLoad();
            UpdateUserDB(); //Update Database when updating game

            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(Global.config.automatic_backup_interval_in_seconds));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                var backUpPath = Global.config.automatic_backup_database_storage_path;
                var databasePath = Global.config.database_path;
                
                File.Copy(databasePath, backUpPath, true);
                Console.WriteLine ("Doing backup...");
            }
            
        }

        /*public static void LoadConfig(){
            Console.WriteLine("loading config : ..." + File.ReadAllText("../config.json"));
            Global.config = 
        }*/
        public static void LoadEnv(){
            var dotEnvLoadStatus = DotEnv.Load("../.env");
            if (dotEnvLoadStatus == false) {
                System.Environment.Exit(1);
            }
        }
        public static void LoadOsuApi(){
            if(!Global.config.dev || false){ //put to true for refreshing osu tokens
                Console.Error.WriteLine("Creating new osu tokens ...");

                var code = "long string"; 
                // for sending messages used for verifications, to get the code, check the following link :
                //https://osu.ppy.sh/oauth/authorize?client_id=422727&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=chat.write
                //replace client_id with your osu client id, same with the redicted_uri
                
                Osu.Api.AuthorizeSelf(code);
                Osu.Api.GetTokens();
                //_ = Global.RunInBackground(TimeSpan.FromSeconds(OsuApi.tokens.expires_in - 3600), OsuApi.RefreshTokens);
            }
            else {
                Console.Error.WriteLine("Loading osu tokens ...");
                if(File.Exists(Global.config.osu_tokens_storage_path))
                    Osu.Api.tokens = JsonConvert.DeserializeObject<OAuthTokens>(File.ReadAllText(Global.config.osu_tokens_storage_path));
                else {
                    Osu.Api.tokens = new OAuthTokens();
                    Console.Error.WriteLine("There is no Osu api tokens found, all features related to osu won't work");
                }
                if(File.Exists(Global.config.osu_chat_tokens_storage_path))
                    Osu.Api.chat_tokens = JsonConvert.DeserializeObject<OAuthTokens>(File.ReadAllText(Global.config.osu_chat_tokens_storage_path));
                else {
                    Console.Error.WriteLine("There is no Osu chat api tokens found, user verification won't work, to make it work, please provide a valid code in the OsuApi.AuthorizeSelf using the link inside the same function");
                    Osu.Api.tokens = new OAuthTokens();
                }
                    
            }
        }
        public static void LoadWebSocketServer(){
            Global.ws = new WebSocketServer(IPAddress.Any, Global.config.ws_port ,false);
            Global.ws.AddWebSocketService<WS> ("/");
            Global.ws.AddWebSocketService<WS> ("/ws");
            Global.ws.Start();

            if (Global.ws.IsListening) {
                Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", Global.ws.Port);

            foreach (var path in Global.ws.WebSocketServices.Paths)
                Console.WriteLine ("- {0}", path);
            }
        }

        public static void FirstLoad(){
            Console.WriteLine("Launching first time configuration ...");
            var waifu = new Waifu()
            {
                name = "Rem",
                imgPATH = "GYrXGACboAACxp7.jpg",
                diffLvlUp = 3,
                id = "0",
                o_str = 10,
                u_str = 2,
                o_kaw = 10,
                u_kaw = 2,
                o_int = 10,
                u_int = 2,
                o_dex = 10,
                u_dex = 2,
                o_agi = 10,
                u_agi = 2,
                o_luck = 10,
                u_luck = 2,
            };
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var waifuCol = db.GetCollection<Waifu>("waifudb");
                waifuCol.Insert(waifu);
                waifuCol.EnsureIndex(x => x.id, true);
            }
            Global.config.first_time_running = false;
            File.WriteAllText("../config.json", JsonConvert.SerializeObject(Global.config));
        }

        public static void UpdateUserDB()
        {
            Console.WriteLine("Updating user database...");
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var userCol = db.GetCollection<UserData.User>("userdb");
                var users = userCol.FindAll();
                //Update
                foreach (UserData.User user in users) {
                    user.inventory ??= new ();
                    user.waifus ??= [];
                    user.verification ??= new();
                    user.pullBannerHistory ??= new Dictionary<string, PullBannerHistory>();
                    if(user.waifu != null)
                    {
                        user.waifus.Add(user.waifu);
                        user.waifu = null;
                    }
                    
                    /*if(user.waifus.id == null){
                        user.waifus.id = "0";
                    }*/
                    //user.waifus.ForEach(waifu => waifu.Update());
                        //Is it still working?
                    //userCol.Update(user);

                }
            }
        }
    }
}
