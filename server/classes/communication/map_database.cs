using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected async void AddMapToDatabase(ClientWebSocketResponse rawData){
        
        var user = DBUtils.GetUser(rawData.id);
        if (user.admin == false ) {Send(ClientNotification.NotificationData("Admin", "You don't have the permissions for this action!", 0)); return;}
        var Beatmap = await OsuApi.GetBeatmapById(rawData.data);
        if(Beatmap == null) {Send(ClientNotification.NotificationData("Admin", "This beatmap doesn't exist!", 1)); return;}
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var mapsCol = db.GetCollection<OsuBeatmap>("osumapsdb");
            Console.WriteLine("mapsCol count : " + mapsCol.Count());
            Console.WriteLine("Beatmap: " + JsonConvert.SerializeObject(Beatmap));
            if(mapsCol.Count() != 0 ){
                if(mapsCol.Count() != 0 && mapsCol.Exists(x => x.id == Beatmap.id)){Send(ClientNotification.NotificationData("Admin", "The beatmap is already in the database!", 1)); return ;} //If the map is already on the data base, don't add it again.
            }
            mapsCol.Insert(Beatmap);
            mapsCol.EnsureIndex(x => x.id, true);
            mapsCol.EnsureIndex(x => x.difficulty_rating);
            Console.WriteLine("Adding map : " + Beatmap.id);
            Send(ClientNotification.NotificationData("Admin", "Added the beatmap into the database!", 3));
        }
    }

}






