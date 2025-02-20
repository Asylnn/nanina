using WebSocketSharp.Server;
using LiteDB;
using Newtonsoft.Json;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.Communication;
using Nanina.UserData.WaifuData;
using System.Net;
using Nanina.UserData.ItemData;

namespace Nanina.Database
{
    public static class LoadServer {

        public static async void Load(){
            //LoadConfig();
            DotEnv.Load("../.env");
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

        public static void LoadOsuApi(){
            if(!Global.config.dev || false){ //put to true for refreshing osu tokens
                Console.Error.WriteLine("Creating new osu tokens ...");

                var code = "long string"; 
                // to get the code used for sending messages used for verifications, check the following link :
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

            /*get waifu col to insert a waifu*/

            DBUtils.Insert(waifu);
            
            Global.config.first_time_running = false;
            File.WriteAllText("../config.json", JsonConvert.SerializeObject(Global.config));
        }

        public static void UpdateUserDB()
        {
            Console.WriteLine("Updating user database...");
            
            /*get user col to update all users*/
            var userCol = DBUtils.GetCollection<UserData.User>();
            var users = userCol.FindAll();
            //Update
            foreach (UserData.User user in users) {

                user.max_energy = Global.baseValues.base_max_energy;
                user.energy = user.max_energy;
                user.isRegenerating = false;
                /*var equipments = user.inventory.equipment;
                user.inventory.equipment = [];
                foreach(var equipment in equipments)
                {
                    equipment.piece = equipment.imgPATH switch {
                        "dress.svg" => EquipmentPiece.Dress, 
                        "ring.svg" => EquipmentPiece.Accessory, 
                        "sword.svg" => EquipmentPiece.Weapon, 
                    };
                    user.inventory.AddEquipment(equipment);
                }
                var waifus = user.waifus; //DO NOT WORK, ERASE ALL WAIFUS
                string[] ids = [];
                user.waifus = [];
                foreach(var waifu in waifus) 
                {
                    if(ids.Contains(waifu.id))
                        continue;
                    ids.Append(waifu.id);
                    user.waifus.Append(waifu);
                }*/
                user.waifus ??= [];
                user.verification ??= new();
                user.pullBannerHistory ??= new Dictionary<string, PullBannerHistory>();
                DBUtils.UpdateUser(user);
                
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
