using WebSocketSharp.Server;
using WebSocketSharp;
public static class LoadServer {

    public static void Load(){
        LoadEnv();
        LoadOsuApi();
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
            OsuApi.tokens = Newtonsoft.Json.JsonConvert.DeserializeObject<OsuOAuthTokens>(File.ReadAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH")));
        }
    }
    public static void LoadWebSocketServer(){
        var ws = new WebSocketServer("ws://localhost:4889");
            ws.AddWebSocketService<Server> ("/");
            ws.AddWebSocketService<Server> ("/test");
            ws.Start();
            if (ws.IsListening) {
                Console.WriteLine ("Listening on port {0}, and providing WebSocket services:", ws.Port);

            foreach (var path in ws.WebSocketServices.Paths)
                Console.WriteLine ("- {0}", path);
            }
            
    }
}