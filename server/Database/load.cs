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

        public static async void LoadOsuApi(){
            bool updateTokens = false;
            bool updateChatTokens = false;

            if(File.Exists(Global.config.osu_tokens_storage_path))
            {
                Osu.Api.tokens = JsonConvert.DeserializeObject<OAuthTokens>(File.ReadAllText(Global.config.osu_tokens_storage_path));
                if(Osu.Api.tokens.expiration_timestamp < Utils.GetTimestamp())
                    updateTokens = true;
            }
            else {
                updateTokens = true;
                //Osu.Api.tokens = new OAuthTokens();
                //Console.Error.WriteLine("There is no Osu api tokens found, all features related to osu won't work");
            }

            if(File.Exists(Global.config.osu_chat_tokens_storage_path))
            {
                Osu.Api.chat_tokens = JsonConvert.DeserializeObject<OAuthTokens>(File.ReadAllText(Global.config.osu_chat_tokens_storage_path));
                if(Osu.Api.chat_tokens.expiration_timestamp < Utils.GetTimestamp())
                    updateChatTokens = true;
            }
            else {
                updateChatTokens = true;
                Console.Error.WriteLine("There is no Osu chat api tokens found, user verification won't work, to make it work, please provide a valid code in the OsuApi.AuthorizeSelf using the link inside the same function");
                //Osu.Api.tokens = new OAuthTokens();
            }
            if(updateTokens){
                Console.Error.WriteLine("Creating new osu tokens ...");
                Osu.Api.GetTokens();
            }

            if(updateChatTokens)
            {
                var code = "def502005fc1d20bfdef0659b7c113a09659a07808740ead64975276a4bcdb594e0be662b5f9f8c2adde72ddcfb8f228699f4eb11a639fab9282aa54a15eab23f812319cebf1d3e0b5014e574d3c7976c5b49fd0222c8353c960ee253bf2f9a8ad15597492540e5f789b932eb763322e83eb919eea094787a3f85f90337f906b7ed4aed1de9e1092dbc271f8c93e44d63685ea36f992620aed8257ab183314285a144b6558d93dd158396f11b0276d32771ef79a2535a06eacf1d082ea05dd9c9dab64425534dd1c8822379e7cdafda203a37bddc4ac89dd2b5dda089e65cd65a62301690471f015987f1575dc9497e97e90495fcc7360dcf217106dc7cfb146f28ebf677877e9bc48153aabe6c1dd05d63bb3f54b77b9bc0ce030f73d7c3a12ab33f9f05a39b7ea1e360a9bbd5c6cce254f4bb7f7cb1b73df9b0ea44dbf8c185f126bac4bdda6d85be7af08eba78956e6e7e730143bdb5f4cbd79c85d7febae1b064f0409cd41294e7e72d2ad28049372fe4925"; 
                // to get the code used for sending messages used for verifications, check the following link :
                //https://osu.ppy.sh/oauth/authorize?client_id=842&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=chat.write
                //https://osu.ppy.sh/oauth/authorize?client_id=842&response_type=code&redirect_uri=https%3A%2F%2Fasyln.moe%2Fnanina&scope=chat.write
                //replace client_id with your osu client id, same with the redicted_uri
                //The code in the message sent for verification is printed in the command line, so for barebone testing purposes it's not necessary.
                Osu.Api.AuthorizeSelf(code);
            }

            var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(68400-600));
            while (await periodicTimer.WaitForNextTickAsync())
            {
                Console.WriteLine("Refreshing osu tokens...");
                Osu.Api.GetTokens();
                Osu.Api.RefreshChatTokens();
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

        public static void UpdateUserDB()
        {
            Console.WriteLine("Updating user database...");
            
            /*get user col to update all users*/
            var userCol = DBUtils.GetCollection<UserData.User>();
            var users = userCol.FindAll();
            //Update
            foreach (UserData.User user in users) {

                user.energy = user.max_energy;
                user.isRegenerating = false;
                //user.inventory.equipment = [];
                //user.activities = [];
                /*foreach(Waifu waifu in user.waifus)
                {
                    waifu.isDoingSomething = false;
                }*/
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
                //user.fightHistory = new();
                /*foreach(var item in user.inventory.userConsumable)
                {
                    var itemDB = BUtils.Get<Item>(x => x.id == item.id);
                    item.rarity = itemDB.rarity;
                    item.modifiers = itemDB.modifiers;
                    item.imgPATH = itemDB.imgPATH;
                }*/
                user.waifus ??= [];
                user.verification ??= new();
                user.pullBannerHistory ??= new Dictionary<string, PullBannerHistory>();
                //user.admin = true;
                DBUtils.Update(user);
                
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
