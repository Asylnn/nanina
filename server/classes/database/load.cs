using WebSocketSharp.Server;
using LiteDB;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
public static class LoadServer {

    public static void Load(){
        LoadEnv();
        LoadOsuApi();
        LoadWebSocketServer();
        if(!File.Exists(Environment.GetEnvironmentVariable("INFO_STORAGE_PATH"))) 
            FirstLoad();
        UpdateUserDB(); //Update Database when updating game
    }
    public static void LoadEnv(){
        var dotEnvLoadStatus = DotEnv.Load("../.env");
        if (dotEnvLoadStatus == false) {
            System.Environment.Exit(1);
        }
    }
    public static void LoadOsuApi(){
        if(Environment.GetEnvironmentVariable("DEV") == "false" || true){ //put to true for refreshing osu tokens
            
            OsuApi.RefreshTokens();
            //_ = Global.RunInBackground(TimeSpan.FromSeconds(OsuApi.tokens.expires_in - 3600), OsuApi.RefreshTokens);
        }
        else {
            //Console.WriteLine("uwu ", File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")));
            OsuApi.tokens = Newtonsoft.Json.JsonConvert.DeserializeObject<OsuOAuthTokens>(File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")));
        }
    }
    public static void LoadWebSocketServer(){
        var ws = new WebSocketServer("ws://localhost:4889");
        ws.AddWebSocketService<WS> ("/");
        ws.AddWebSocketService<WS> ("/test");
        ws.Start();
        if (ws.IsListening) {
            Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", ws.Port);

        foreach (var path in ws.WebSocketServices.Paths)
            Console.WriteLine ("- {0}", path);
        }
    }

    public static void FirstLoad(){
        var waifu = new PocoWaifu()
        {
            name = "Rem",
            xp = 0,
            lvl = 1,
            imgPATH = "GYrXGACboAACxp7.jpg",
            diffLvlUp = 3,
            id = "0"
        };
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var waifuCol = db.GetCollection<PocoWaifu>("waifudb");
            waifuCol.Insert(waifu);
            waifuCol.EnsureIndex(x => x.id, true);
        }
        var info = new Info{
            first_time_running = false
        };
        File.WriteAllText(Environment.GetEnvironmentVariable("INFO_STORAGE_PATH"), Newtonsoft.Json.JsonConvert.SerializeObject(info));
    }

    public static void UpdateUserDB()
    {
         using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var userCol = db.GetCollection<PocoUser>("userdb");
            var users = userCol.FindAll();
                Console.WriteLine("hey1");
            
            //Update
            foreach (PocoUser user in users) {
                user.fights = [];
                Console.WriteLine("hey2");
                Console.WriteLine(user.waifu.id);
                if(user.waifu.id == null){
                Console.WriteLine("hey3");

                    user.waifu.id = "0";
                }
                Waifu.UpdateWaifu(user.waifu);
                userCol.Update(user);

            }
        }
    }
}