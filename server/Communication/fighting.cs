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
            var map = DBUtils.GetMap(rawData.data);
            if(map == null)
                {Send(ClientNotification.NotificationData("Fighting", "Couldn't get the map??", 1)); return ;}

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "map link",
                data = JsonConvert.SerializeObject(map)
            }));
        }
        protected void GetChartToFight(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.timestamp + Global.baseValues.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }
            
            var charts = Global.charts.Where(chart => chart.levelNum >= 10);
            var chart = charts.RandomElement();

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "maimai link",
                data = JsonConvert.SerializeObject(chart)
            }));
            user.fight = new Fight 
            {
                game = "maimai",
                timestamp = Utils.GetTimestamp(),
                id = chart.songID.ToString(),
                completed = false
            };

            DBUtils.UpdateUser(user);
        }

        protected async void ClaimChart(ClaimClientResponse claim, UserData.User user)
        {

            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.completed)
                { Send(ClientNotification.NotificationData("Fighting", "You completed the last fight! You need to start a new one!", 0)); return; }
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
            if(!user.verification.isMaimaiTokenVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return; }
            var waifu = user.waifus.Find(waifus => waifus.id == claim.id);
            if(waifu == null)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't choose a waifu to XP!", 0)); return; }

            var scores = await Maimai.Api.GetRecentScores(user.tokens.maimai_token, Convert.ToInt16(user.fight.id), 0);

            user.claimTimestamp = Utils.GetTimestamp();

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Count() == 0) 
                { Send(ClientNotification.NotificationData("Fighting", "Did you do the chart?", 3)); return; }                    
            //Console.WriteLine(JsonConvert.SerializeObject(scores));
            var validscore = Array.Find(scores, score => user.fight.id == score.song.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "Did you do the chart?", 0)); return;
            }
            
            var spent_energy = user.SpendEnergy();
            
            var xp = (uint) Math.Ceiling(Maimai.Api.GetXP(validscore)*spent_energy);
            waifu.GiveXP(xp);
            user.fight.completed = true;
            if(user.fightHistory.ContainsKey(user.fight.game))
                user.fightHistory[user.fight.game].Append(user.fight.id);
            else
                user.fightHistory.Add(user.fight.game, [user.fight.id]);
            
            user.statCount.maimai_claim_count++;
            DBUtils.UpdateUser(user);
            UserData.User.RegenEnergy(user);

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "fighting results",
                data = xp.ToString()
            }));


            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
            

        }
              
        
        protected void GetMapToFight(ClientWebSocketResponse rawData){
            if(rawData.data == "maimai")                //TO MODIFY!!
                {GetChartToFight(rawData); return;}


            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.timestamp + Global.baseValues.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }                    
            
            var mapsCol = DBUtils.GetCollection<Beatmap>();
            var maps = mapsCol.Find(x => x.difficulty_rating <= 7.27*2.7);
            if(maps.Count() == 0)
                {Console.WriteLine("There isn't any map in the database!!!"); return;}
            var map = maps.RandomElement();
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "map link",
                data = JsonConvert.SerializeObject(map)
            }));
            user.fight = new Fight 
            {
                game = map.mode,
                timestamp = Utils.GetTimestamp(),
                id = map.id.ToString(),
                completed = false
            };

            
        }

        protected class ClaimClientResponse
        {
            public string id;
            public string game;
        }
        protected async void ClaimFight(ClientWebSocketResponse rawData){

            var claim = JsonConvert.DeserializeObject<ClaimClientResponse>(rawData.data);
            var user = DBUtils.GetUser(rawData.userId);

            if(claim.game == "maimai")                //TO MODIFY!!
                {ClaimChart(claim, user); return;}


            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.completed)
                { Send(ClientNotification.NotificationData("Fighting", "You completed the last fight! You need to start a new one!", 0)); return; }
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
            if(!user.verification.isOsuIdVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return; }
            var waifu = user.waifus.Find(waifus => waifus.id == claim.id);
            if(waifu == null)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't choose a waifu to XP!", 0)); return; }

            var scores = await Osu.Api.GetUserRecentScores(user.ids.osuId, user.fight.game);

            user.claimTimestamp = Utils.GetTimestamp();

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Count() == 0) 
                { Send(ClientNotification.NotificationData("Fighting", "You don't have any recent scores! (OR osu api keys are expired)", 3)); return; }                    
            //Console.WriteLine(JsonConvert.SerializeObject(scores));
            var validscore = Array.Find(scores, score => user.fight.id == score.beatmap.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "You didn't do the beatmap (must be a pass)", 0)); return;
            }
            
            var spent_energy = user.SpendEnergy();
            var xp = (uint) Math.Ceiling(Osu.Api.GetXP(validscore)*spent_energy);
            waifu.GiveXP(xp);
            user.fight.completed = true;
            if(user.fightHistory.ContainsKey(user.fight.game))
                user.fightHistory[user.fight.game].Append(user.fight.id);
            else
                user.fightHistory.Add(user.fight.game, [user.fight.id]);

            user.statCount.std_claim_count++;
            DBUtils.UpdateUser(user);
            UserData.User.RegenEnergy(user);
            
            
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "fighting results",
                data = xp.ToString()
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
