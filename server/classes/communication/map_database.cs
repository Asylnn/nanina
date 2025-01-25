using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected async void AddMapToDatabase(ClientWebSocketResponse rawData){
        
        var user = DBUtils.GetUser(rawData.id);
        if (user.admin == false ) {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
        var Beatmap = await OsuApi.GetBeatmapById(rawData.data);
        if(Beatmap == null) {Send(ClientNotification.NotificationData("admin", "this beatmap doesn't exist!", 1)); return;}
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var mapsCol = db.GetCollection<OsuBeatmap>("osumapsdb");
            if(mapsCol.Exists(x => x.id == Beatmap.id)){Send(ClientNotification.NotificationData("admin", "the beatmap is already in the database!", 1)); return ;} //If the map is already on the data base, don't add it again.
            mapsCol.Insert(Beatmap);
            mapsCol.EnsureIndex(x => x.id, true);
            mapsCol.EnsureIndex(x => x.difficulty_rating);
            Console.WriteLine("Adding map : " + Beatmap.id);
            Send(ClientNotification.NotificationData("admin", "added the beatmap into the database!", 3));
        }
    }

}






