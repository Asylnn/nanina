using Nanina.Database;
using Nanina.Dungeon;
using Nanina.Osu;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        #pragma warning disable 0649
        protected class StartDungeonFormat
        {
            public string? id;
            public string[]? waifuIds;
            public byte floor;
        }
        #pragma warning restore 0649
        protected void StartDungeon(ClientWebSocketResponse rawData){
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}

            var clientData = JsonConvert.DeserializeObject<StartDungeonFormat>(rawData.data);
            if(clientData == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data (cliendData is null)", 1)); return ;}
            if(clientData.id == null || clientData.waifuIds == null || clientData.floor <= 0 || clientData.floor > 5)
                {Send(ClientNotification.NotificationData("User", "Invalid data (clientData.id or clientData.waifuIds are null, or clientData.floor is not between 1 and 5)", 1)); return ;}

            var dungeonList = DungeonManager.dungeons.Where(dungeon => dungeon.id == clientData.id);
            if(dungeonList.Count() == 0)
                {Send(ClientNotification.NotificationData("Dungeon", "The dungeon you tried to start doesn't exist!", 1)); return ;}
            var dungeon = dungeonList.First();
            
            var waifus = user.waifus.Where(waifu => clientData.waifuIds.Contains(waifu.id)).ToList();
            if(waifus.Count() != 3)
                {Send(ClientNotification.NotificationData("User", "Invalid data (at least one invalid waifu)", 1)); return ;}

            if(waifus.Any(waifu => waifu.isDoingSomething))
                {Send(ClientNotification.NotificationData("User", "One of the waifu is doing something", 1)); return ;}
            
            //In case any dungeon is still in progress somehow? Maybe break too much the performance ?
            var activeDungeons = DungeonManager.activeDungeons.Values.ToList().Where(dungeon => dungeon.userId == user.Id);
            foreach(ActiveDungeon activeDungeon in activeDungeons)
                activeDungeon.StopDungeon();

            DungeonManager.InstantiateDungeon(dungeon, user, waifus, session.id, clientData.floor);
            
            foreach(var waifu in waifus)
            {
                waifu.isDoingSomething = true;
            }
            user.waifuIdsInDungeon = clientData.waifuIds.ToList();
            user.isInDungeon = true;
            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user)
            }));

            /*
                The data of the new dungeon is sent to the client inside the InstantiateDungeon method
            */

            
        }

        protected void StopDungeon(ClientWebSocketResponse rawData){
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}

            var dungeon = DungeonManager.activeDungeons[Convert.ToUInt64(rawData.data)];
            if(dungeon == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You are not in a dungeon", 1)); return ;}
            // I don't know if this is trully necessary but It's just costing me a few minutes anyways
            if(user.dungeonInstanceId != dungeon.instanceId)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't stop someone's else dungeon", 1)); return ;}

            user.isInDungeon = false;
            DBUtils.Update(user); //Since StopDungeon also modify user, this update should be before stopDungeon.
            dungeon.StopDungeon();
            
        }


        protected async void ClaimDungeonFight(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}
            if(user.claimTimestamp + Global.baseValues.time_for_allowing_another_claim_in_milliseconds >= Utils.GetTimestamp()) 
                { Send(ClientNotification.NotificationData("Fighting", "You did a claim too recently", 1)); return; }

            var score = await CheckForOsuStandardScores(user);
            if(score is null) return;
            var mult = Osu.Api.GetDungeonMult(score);

            var dungeon = DungeonManager.activeDungeons[Convert.ToUInt64(rawData.data)];
            dungeon.DealDamage(mult);
            DungeonManager.UpdateDungeonOfClient(dungeon);
        }
    }
}
