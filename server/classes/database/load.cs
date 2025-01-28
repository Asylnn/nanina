using WebSocketSharp.Server;
using LiteDB;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;
public static class LoadServer {

    public static void Load(){
        LoadConfig();
        LoadEnv();
        LoadOsuApi();
        LoadWebSocketServer();
        Gacha.LoadBanners();
        if(!File.Exists(Environment.GetEnvironmentVariable("INFO_STORAGE_PATH"))) 
            FirstLoad();
        UpdateUserDB(); //Update Database when updating game

        
    }

    public static void LoadConfig(){
        Console.WriteLine("loading config : ..." + File.ReadAllText("../config.json"));
        Global.config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../config.json"));
    }
    public static void LoadEnv(){
        var dotEnvLoadStatus = DotEnv.Load("../.env");
        if (dotEnvLoadStatus == false) {
            System.Environment.Exit(1);
        }
    }
    public static void LoadOsuApi(){
        if(Environment.GetEnvironmentVariable("DEV") == "false" || false){ //put to true for refreshing osu tokens
            var code = "long string"; 
            // for sending messages used for verifications, to get the code, check the following link :
            //https://osu.ppy.sh/oauth/authorize?client_id=422727&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=chat.write
            //replace client_id with your osu client id, same with the redicted_uri
            
            OsuApi.AuthorizeSelf(code);
            OsuApi.RefreshTokens();
            //_ = Global.RunInBackground(TimeSpan.FromSeconds(OsuApi.tokens.expires_in - 3600), OsuApi.RefreshTokens);
        }
        else {
            //Console.WriteLine("uwu ", File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")));
            if(File.Exists(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")))
                OsuApi.tokens = JsonConvert.DeserializeObject<OsuOAuthTokens>(File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")));
            else {
                OsuApi.tokens = new OsuOAuthTokens();
                Console.Error.WriteLine("There is no Osu api tokens found, all features related to osu won't work");
            }
            if(File.Exists(Environment.GetEnvironmentVariable("OSU_API_CHAT_TOKEN_STORAGE_PATH")))
                OsuApi.chat_tokens = JsonConvert.DeserializeObject<OsuOAuthTokens>(File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_CHAT_TOKEN_STORAGE_PATH")));
            else {
                Console.Error.WriteLine("There is no Osu chat api tokens found, user verification won't work, to make it work, please provide a valid code in the OsuApi.AuthorizeSelf using the link inside the same function");
                OsuApi.tokens = new OsuOAuthTokens();
            }
                
        }
    }
    public static async void LoadWebSocketServer(){
        var ws = new WebSocketServer("ws://localhost:4889");
        ws.AddWebSocketService<WS> ("/");
        ws.AddWebSocketService<WS> ("/test");
        ws.Start();

        if (ws.IsListening) {
            Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", ws.Port);

        foreach (var path in ws.WebSocketServices.Paths)
            Console.WriteLine ("- {0}", path);
        }

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(Convert.ToDouble(Environment.GetEnvironmentVariable("AUTOMATIC_BACKUP_TIME"))));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            var backUpPath = Environment.GetEnvironmentVariable("AUTOMATIC_BACKUP_STORAGE_PATH");
            var databasePath = Environment.GetEnvironmentVariable("DATABASE_PATH");
            
            File.Copy(databasePath, backUpPath, true);
            Console.WriteLine ("Backup...");
        }
    }

    public static void FirstLoad(){
        var waifu = new Waifu()
        {
            name = "Rem",
            xp = 0,
            lvl = 1,
            imgPATH = "GYrXGACboAACxp7.jpg",
            diffLvlUp = 3,
            id = "0"
        };
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var waifuCol = db.GetCollection<Waifu>("waifudb");
            waifuCol.Insert(waifu);
            waifuCol.EnsureIndex(x => x.id, true);
        }
        var info = new Info{
            first_time_running = false
        };
        File.WriteAllText(Environment.GetEnvironmentVariable("INFO_STORAGE_PATH"), JsonConvert.SerializeObject(info));
    }

    public static void UpdateUserDB()
    {
         using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var userCol = db.GetCollection<User>("userdb");
            var users = userCol.FindAll();
            //Update
            foreach (User user in users) {
                user.fights = [];
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
                user.waifus.ForEach(waifu => waifu.Update());
                    //Is it still working?
                userCol.Update(user);

            }
        }
    }
}