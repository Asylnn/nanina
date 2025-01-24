using WebSocketSharp.Server;
using LiteDB;
public static class LoadServer {

    public static void Load(){
        LoadEnv();
        LoadOsuApi();
        LoadWebSocketServer();
        UpdateUserDB(); //Update Database when updating game
    }
    public static void LoadEnv(){
        var dotEnvLoadStatus = DotEnv.Load("../.env");
        if (dotEnvLoadStatus == false) {
            System.Environment.Exit(1);
        }
    }
    public static void LoadOsuApi(){
        if(Environment.GetEnvironmentVariable("DEV") == "false" || false){ //put to true for refreshing osu tokens
            
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

    public static void UpdateUserDB()
    {
         using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var user_col = db.GetCollection<PocoUser>("userdb");
            var users = user_col.FindAll();
            
            //Update
            foreach (PocoUser user in users) {
                user.fights = [];
                user_col.Update(user);
            }
        }
    }
}