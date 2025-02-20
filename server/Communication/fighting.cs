using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Osu;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using System.Threading.Tasks;

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
        protected void GetMaimaiChartToFight(UserData.User user)
        {
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
                id = chart.songID.ToString(),
            };

            DBUtils.UpdateUser(user);
        }

        protected async Task<uint> CheckForMaimaiScores(UserData.User user)
        {

            if(!user.verification.isMaimaiTokenVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return 0; }

            var scores = await Maimai.Api.GetRecentScores(user.tokens.maimai_token, Convert.ToInt16(user.fight.id), 0);
            user.claimTimestamp = Utils.GetTimestamp();

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Count() == 0) 
                { Send(ClientNotification.NotificationData("Fighting", "Did you do the chart?", 3)); return 0; }                    
            //Console.WriteLine(JsonConvert.SerializeObject(scores));
            var validscore = Array.Find(scores, score => user.fight.id == score.song.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "Did you do the chart?", 0)); return 0;
            }
            
            return Maimai.Api.GetXP(validscore);
        }
              
        
        protected void StartFight(ClientWebSocketResponse rawData){

            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.timestamp + Global.baseValues.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }
            
            if(rawData.data == "maimai")                
                GetMaimaiChartToFight(user);
            else
                GetMapToFight(user);
        }

        protected async void ClaimFight(ClientWebSocketResponse rawData){

            var claim = JsonConvert.DeserializeObject<ClaimClientResponse>(rawData.data);
            var user = DBUtils.GetUser(rawData.userId);

            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.fight.completed)
                { Send(ClientNotification.NotificationData("Fighting", "You completed the last fight! You need to start a new one!", 0)); return; }
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
            
            var waifu = user.waifus.Find(waifus => waifus.id == claim.id);
            if(waifu == null)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't choose a waifu to XP!", 0)); return; }
                
            var baseXP = 0u;
            if(claim.game == "maimai")                
                baseXP = await CheckForMaimaiScores(user);
            else
                baseXP = await CheckForOsuStandardScores(user);

            if(baseXP == 0) return;

            var (spent_energy, gc) = user.SpendEnergy();
            var xp = (uint) Math.Ceiling(baseXP*spent_energy);
            waifu.GiveXP(xp);
            user.fight.completed = true;
            if(user.fightHistory.ContainsKey(user.fight.game))
                user.fightHistory[user.fight.game].Append(user.fight.id);
            else
                user.fightHistory.Add(user.fight.game, [user.fight.id]);

            user.statCount.std_claim_count++;
            

            SendLoot([
                new Loot {
                    lootType = LootType.WaifuXP,
                    amount = xp,
                },
                new Loot {
                    lootType = LootType.GC,
                    amount = gc,
            }]);
            
            
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
            UserData.User.RegenEnergy(user);
        }
        
        protected void GetMapToFight(UserData.User user)
        {          
            

            /* Get col de beatmap pour find maps
            If maps empty, return
            If maps not empty, pick a random one then update user.fight with said map and send websocket
            then, update user with the changed user.fight
            */
            var map = DBUtils.Get<Beatmap>(x => x.difficulty_rating <= 7.27*2.7,true);
            if(map is null)
                {Console.WriteLine("There isn't any map in the database!!!"); return;}
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "map link",
                data = JsonConvert.SerializeObject(map)
            }));
            user.fight = new Fight 
            {
                game = map.mode,
                id = map.id.ToString(),
            };

            DBUtils.Update(user);
        }

        protected class ClaimClientResponse
        {
            public string id;
            public string game;
        }
        protected async Task<uint> CheckForOsuStandardScores(UserData.User user){

            if(!user.verification.isOsuIdVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your osu account! Go to the settings and enter your osu id!", 0)); return 0; }
            
            user.claimTimestamp = Utils.GetTimestamp();
            var scores = await Osu.Api.GetUserRecentScores(user.ids.osuId, user.fight.game);

            

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Count() == 0) 
                { Send(ClientNotification.NotificationData("Fighting", "You don't have any recent scores! (OR osu api keys are expired)", 3)); return 0; }                    
            //Console.WriteLine(JsonConvert.SerializeObject(scores));
            var validscore = Array.Find(scores, score => user.fight.id == score.beatmap.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "You didn't do the beatmap (must be a pass)", 0)); return 0;
            }
            
            return Osu.Api.GetXP(validscore);
        }
    }
}
