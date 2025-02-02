using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;

partial class WS : WebSocketBehavior
{
    protected void SendMapToClient(ClientWebSocketResponse rawData){
        using var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}");
        var mapsCol = db.GetCollection<OsuBeatmap>("osumapsdb");
        var maps = mapsCol.Find(x => x.id == Convert.ToInt64(rawData.data));
        var mapstest = mapsCol.Find(x => true).First();
        
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "map link",
            data = JsonConvert.SerializeObject(maps.First())
        }));
    }
    public void GetMapToFight(ClientWebSocketResponse rawData){ //somehow protected doesn't work?
        var user = DBUtils.GetUser(rawData.id);
        if(user.fights.Count() != 0 && user.fights.Last().timestamp + Global.config.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }                    
        
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var mapsCol = db.GetCollection<OsuBeatmap>("osumapsdb");
            var maps = mapsCol.Find(x => x.difficulty_rating <= 7.27*2.7);
            if(maps.Count() == 0){Console.WriteLine("There isn't any map in the database!!!"); return;} //Peut etre faire un if else ici?
            Random rng = new ();
            var random_elem = rng.Next(maps.Count());
            var map = maps.ElementAt(random_elem);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "map link",
                data = JsonConvert.SerializeObject(map)
            }));
            user.fights.Add(new Fight {
                game = map.mode,
                timestamp = Utils.GetTimestamp(),
                id = map.id.ToString()
            });

            DBUtils.UpdateUser(user);
        }
    }
    protected async void ClaimFight(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        
        if(user.fights.Last().completed) { Send(ClientNotification.NotificationData("Fighting", "You completed the last fight! You need to start a new one!", 0)); return; }
        if(user.claimTimestamp + Global.config.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
        if(!user.verification.isOsuIdVerified) { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return; }

        var scores = await OsuApi.GetUserRecentScores(user.ids.osuId, user.fights.Last().game);

        user.claimTimestamp = Utils.GetTimestamp();
        DBUtils.UpdateUser(user);

        //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a frodulent Websocket with mrekk id set as their id
        if(scores.Count() == 0) { Send(ClientNotification.NotificationData("Fighting", "You don't have any recent scores OR you didn't entered your osu id in the options! (OR osu api keys are expired)", 3)); return; }                    
        if(user.fights.Count() == 0) { Send(ClientNotification.NotificationData("Fighting", "Somehow you don't have any fight object??", 0)); return; }
        //This shouldn't happen... right??

        
        var validscore = Array.Find(scores, score => user.fights.Any(fight => fight.id == score.beatmap.id.ToString()));


        if(validscore == null){
            Console.WriteLine($"There wasn't any valid score found for {user.fights.First().id} (Did you do the beatmap?)");
            Send(ClientNotification.NotificationData("Fighting", "Did you do the beatmap?", 0)); return;
        }
        
        var xp = OsuApi.GetXP(validscore);
        user.waifus.First().GiveXP(xp);
        user.fights.Last().completed = true;
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "fighting results",
            data = JsonConvert.SerializeObject(xp) 
        }));


        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "user",
            data = JsonConvert.SerializeObject(user) 
        }));
        

        DBUtils.UpdateUser(user);
    }
}