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
        #pragma warning disable 0649
        protected class ClaimClientResponse
        {
            public string? waifuId;
            public Game game;
        }

        #pragma warning restore 0649
        protected void SendMapToClient(ClientWebSocketResponse rawData)
        {
            var map = DBUtils.Get<Beatmap>(x => x.id == Convert.ToInt64(rawData.data));
            if(map == null)
                {Send(ClientNotification.NotificationData("Fighting", "Couldn't get the map??", 1)); return ;}

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideMapData,
                data = JsonConvert.SerializeObject(map)
            }));
        }
        protected void GetMaimaiChartToFight(UserData.User user)
        {
            var charts = Global.charts.Where(chart => chart.levelNum >= 10);
            var chart = charts.RandomElement();

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideMaimaiChartData,
                data = JsonConvert.SerializeObject(chart)
            }));
            user.fight = new Fight
            {
                game = Game.MaimaiFinale,
                id = chart.songID.ToString(),
                secondaryId = chart.difficultyNum.ToString(),
            };

            /*Console.WriteLine(user.fight.id);
            Console.WriteLine(chart.songID);
            Console.WriteLine(chart.songID.ToString());*/
            DBUtils.Update(user);

            /*var user2 = DBUtils.Get<UserData.User>(x => x.Id == user.Id);
            Console.WriteLine(user2.fight.id);*/
        }

        protected async Task<(uint xp, double ratio)> CheckForMaimaiScores(UserData.User user)
        {
            var scores = await Maimai.Api.GetRecentScores(user.tokens.maimai_token!, Convert.ToUInt32(user.fight!.id), Convert.ToByte(user.fight.secondaryId));
            user.claimTimestamp = Utils.GetTimestamp();

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Length == 0) 
                { Send(ClientNotification.NotificationData("Fighting", "Did you do the chart?", 3)); return (0,1); }                    

            var validscore = Array.Find(scores, score => user.fight.id == score.song.id.ToString());


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight.id} (Did you do the beatmap?)");
                return (0,1);
            }
            /*Console.WriteLine(validscore.play_date_unix);
            Console.WriteLine(Utils.GetTimestamp());
            if(validscore.play_date_unix*1000 + Global.baseValues.maimai_score_expiration_in_milliseconds <= Utils.GetTimestamp())
                { Send(ClientNotification.NotificationData("Fighting", "You did the chart too long ago!", 0)); return (0,1);}*/
            
            return (Maimai.Api.GetXP(validscore), 1);
        }
              
        
        protected void StartFight(ClientWebSocketResponse rawData){

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return ;}
            if(user.fight is not null && user.fight.timestamp + Global.baseValues.time_for_allowing_another_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You have a too much recent fight", 1)); return; }
            
            if((Game) Convert.ToInt16(rawData.data) == Game.MaimaiFinale)                
                GetMaimaiChartToFight(user);
            else
                GetMapToFight(user);
        }

        protected async void ClaimFight(ClientWebSocketResponse rawData){

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);

            if(user is null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return ;}
            if(user.fight is null)
                { Send(ClientNotification.NotificationData("Fighting", "You are not doing any fights", 0)); return; }
            if(Utils.TryDeserialize<ClaimClientResponse>(rawData.data, out var claim) == false) 
                { Send(ClientNotification.NotificationData("Fighting", "Error processing the claim", 1)); return; }
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }
            
            
            if(user.waifus.TryGet(claim.waifuId, out var waifu) == false)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't choose a waifu to XP!", 0)); return; }
                
            var baseXP = 0u;
            var ratio = 1d;
            if(claim.game == Game.MaimaiFinale)
            {
                if(!user.verification.isMaimaiTokenVerified) 
                    { Send(ClientNotification.NotificationData("Fighting", "You didn't verified your maimai account! Go to the settings and enter your maimai access token!", 0)); return; }
                (baseXP, ratio) = await CheckForMaimaiScores(user);
            }
            else
            {
                var score = await CheckForOsuStandardScores(user);
                if(score == null)
                    (baseXP, ratio) = (0,1);
                else
                    (baseXP, ratio) = (Osu.Api.GetXP(score), score.beatmap.hit_length/240f);
            }

            if(baseXP == 0) return;

            var (spent_energy, gc) = user.SpendEnergy(ratio);
            var xp = (int) Math.Ceiling(baseXP*spent_energy/25);
            waifu.GiveXP(xp);
            
            if(user.fightHistory.TryGet(user.fight.game, out var fightHistory))
                fightHistory.Add(user.fight.id);
            else
                user.fightHistory.Add(user.fight.game, [user.fight.id]);

            user.fight = null;
            user.statCount.std_claim_count++;
            user.GetXP(Global.baseValues.user_xp_for_fights);

            SendLoot([
                new Loot {
                    lootType = LootType.WaifuXP,
                    amount = xp,
                },
                new Loot {
                    lootType = LootType.GC,
                    amount = gc,
                },
                new Loot {
                    lootType = LootType.UserXP,
                    amount = Global.baseValues.user_xp_for_fights,
            }], true);

            var dataToClient = new
            {
                xp,
                waifuId = waifu.id,
            };
            
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.GiveXPToWaifu,
                data = JsonConvert.SerializeObject(dataToClient)
            }));

            DBUtils.Update(user);
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
            {
                Console.WriteLine("There isn't any map in the database!!!");
                Send(ClientNotification.NotificationData("User", "There isn't any map in the database!!", 1)); 
                return ;
            }
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideMapData,
                data = JsonConvert.SerializeObject(map)
            }));
            user.fight = new Fight 
            {
                game = Game.OsuStandard,
                id = map.id.ToString(),
            };

            DBUtils.Update(user);
        }

        
        protected async Task<ScoreExtended?> CheckForOsuStandardScores(UserData.User user){

            if(!user.verification.isOsuIdVerified) 
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verify your osu account! Go to the settings and enter your osu id!", 0)); return null; }
            
            user.claimTimestamp = Utils.GetTimestamp();
            var scores = await Osu.Api.GetUserRecentScores(user.ids.osuId!, "osu");

            

            //Ideally, the user shouldn't be able to see the page, but in any case this should stay in case the user is able to send a fraudulent Websocket with mrekk id set as their id
            if(scores.Length == 0)
                { Send(ClientNotification.NotificationData("Fighting", "You don't have any recent scores! (OR osu api keys are expired)", 3)); return null; }                    

            //var validscore = Array.Find(scores, score => user.fight.id == score.beatmap.id.ToString());
            
            var validscore = scores[0];


            if(validscore == null){
                Console.WriteLine($"There wasn't any valid score found for {user.fight?.id} (Did you do the beatmap?)");
                Send(ClientNotification.NotificationData("Fighting", "You didn't do the beatmap (must be a pass)", 0)); return null;
            }
            
            return validscore;
        }

        protected async void CheckContinuousFight(ClientWebSocketResponse rawData)
        {

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if (user is null)
                { Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return; }
            var activeActivities = user.activities.Where(acitivity => !acitivity.finished);
            if (!activeActivities.Any())
                { Send(ClientNotification.NotificationData("User", "You need to have at least a single valid activity in progress", 1)); return; }
            if(user.lastContinuousFightTimestamp + Global.baseValues.time_for_allowing_another_continuous_fight_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a continuous fight too recently", 1)); return; }
            if (!user.verification.isOsuIdVerified)
                { Send(ClientNotification.NotificationData("Fighting", "You didn't verify your osu account! Go to the settings and enter your osu id!", 0)); return; }

            var allScores = await Api.GetUserRecentScores(user.ids.osuId!, "osu", "10");
            var scores = allScores.Where(score => score.beatmap.status == "ranked" && ! user.continuousFightLog.Any(log => log.scoreId == score.id));
            var scoresDTO = scores.ToList().ConvertAll(score => score.ToDTO());

            var totalTimeSave = 0d;
            var scoreCount = scores.Count();
            for (int i = 0; i < scoreCount; i++)
            {
                var xp = Api.GetXP(scores.ElementAt(i)) * 1000;
                var timesave = Math.Ceiling(Math.Pow(xp, 0.75));
                totalTimeSave += timesave;
                scoresDTO[i].timesave = (int) timesave;
            }

            totalTimeSave *= 1000;
            totalTimeSave /= activeActivities.Count();
            foreach(var activity in activeActivities)
            {
                if(Global.activityTimers.TryGetValue(activity.id, out var timer))
                {
                    activity.timeout = (long) Math.Max(0, activity.timeout - totalTimeSave);
                    //To avoid integer overflow
                    if(activity.timestamp + activity.timeout + 100 <= Utils.GetTimestamp())
                        timer.Interval = 100;
                        
                    else
                        //since setting the interval restarts the timer from zero.
                        timer.Interval = activity.timestamp + activity.timeout - Utils.GetTimestamp();
                }
                else
                {
                    Console.Error.WriteLine($"Activity {activity.id} doesn't have a timer");
                }
            }
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideContinuousFightResults,
                data = JsonConvert.SerializeObject(scoresDTO) 
            }));
            var loot = new List<Loot>(
            [ 
                new Loot() {
                    lootType = LootType.TimeSave,
                    amount = (int)totalTimeSave,
                }
            ]);
            
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideLoot,
                data = JsonConvert.SerializeObject(loot)
            }));

            List<ContinuousFightLog> remover = [];
            foreach(var log in user.continuousFightLog)
            {
                if(log.expirationTimestamp <= Utils.GetTimestamp())
                    remover.Add(log);
            }
            user.continuousFightLog.RemoveAll(remover.Contains);
            user.lastContinuousFightTimestamp = Utils.GetTimestamp();
            user.statCount.continuous_fight_count += scoreCount;
            /*foreach(var score in scores)
            {
                user.continuousFightLog.Add(new()
                {
                    scoreId = score.id,
                    expirationTimestamp = Utils.GetTimestamp() + Global.baseValues.continuous_fight_score_expiration_time_in_milliseconds
                });
            }*/
            DBUtils.Update(user);
        }
    }
}
