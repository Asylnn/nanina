using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Osu;
using Nanina.UserData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void SendMapToClient(ClientWebSocketResponse rawData)
        {
            using var db = new LiteDatabase($@"{Global.config.database_path}");
            var mapsCol = db.GetCollection<Beatmap>("osumapsdb");
            var maps = mapsCol.Find(x => x.id == Convert.ToInt64(rawData.data));
            if(maps.Count() != 0)
            {
                Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = "map link",
                    data = JsonConvert.SerializeObject(maps.First())
                }));
            }
        }
        protected void GetMapToFight(ClientWebSocketResponse rawData){ //somehow protected doesn't work?
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.timestamp + Global.baseValues.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }                    
            
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var mapsCol = db.GetCollection<Beatmap>("osumapsdb");
                var maps = mapsCol.Find(x => x.difficulty_rating <= 7.27*2.7);
                if(maps.Count() == 0){Console.WriteLine("There isn't any map in the database!!!"); return;} //Peut etre faire un if else ici?
                Random rng = new ();
                var map = maps.ElementAt(rng.Next(maps.Count()));
                Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = "map link",
                    data = JsonConvert.SerializeObject(map)
                }));
                user.fight = new Fight {
                    game = map.mode,
                    timestamp = Utils.GetTimestamp(),
                    id = map.id.ToString(),
                    completed = false
                };

                DBUtils.UpdateUser(user);
            }
        }
        protected async void ClaimFight(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.completed) 
                { Send(ClientNotification.NotificationData("Fighting", "You completed the last fight! You need to start a new one!", 0)); return; }
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
            if(!user.verification.isOsuIdVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return; }
            var waifu = user.waifus.Find(waifus => waifus.id == rawData.data);
            if(waifu == null)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't choose a waifu to XP!", 0)); return; }

            var scores = await Osu.Api.GetUserRecentScores(user.ids.osuId, user.fight.game);

            user.claimTimestamp = Utils.GetTimestamp();

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a frodulent Websocket with mrekk id set as their id
            if(scores.Count() == 0) { Send(ClientNotification.NotificationData("Fighting", "You don't have any recent scores OR you didn't entered your osu id in the options! (OR osu api keys are expired)", 3)); return; }                    
            //Console.WriteLine(JsonConvert.SerializeObject(scores));
            var validscore = Array.Find(scores, score => user.fight.id == score.beatmap.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "Did you do the beatmap?", 0)); return;
            }
            
            var xp = Osu.Api.GetXP(validscore);
            waifu.GiveXP(xp);
            user.fight.completed = true;
            if(user.fightHistory.ContainsKey(user.fight.game))
            {
                var history = user.fightHistory[user.fight.game];
                history.Append(user.fight.game);
            }
            else
            {
                user.fightHistory.Add(user.fight.game, [user.fight.id]);
            }
            
            
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
}
