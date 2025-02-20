using Nanina.Database;
using Nanina.Dungeon;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected class StartDungeonFormat
        {
            public string id;
            public string[] waifuIds;
        }
        protected void StartDungeon(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.GetSession(rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}

            var clientData = JsonConvert.DeserializeObject<StartDungeonFormat>(rawData.data);
            if(clientData == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data (cliendData is null)", 1)); return ;}
            if(clientData.id == null || clientData.waifuIds == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data (clientData.id or clientData.waifuIds are null)", 1)); return ;}

            var dungeonList = DungeonManager.dungeons.Where(dungeon => dungeon.id == clientData.id);
            if(dungeonList.Count() == 0)
                {Send(ClientNotification.NotificationData("Dungeon", "The dungeon you tried to start doesn't exist!", 1)); return ;}
            var dungeon = dungeonList.First();
            
            var waifus = user.waifus.Where(waifu => clientData.waifuIds.Contains(waifu.id)).ToList();
            if(waifus.Count() != 3)
                {Send(ClientNotification.NotificationData("User", "Invalid data (at least one invalid waifu)", 1)); return ;}

            DungeonManager.InstantiateDungeon(dungeon, user, waifus, session.id);
        }

        protected void StopDungeon(ClientWebSocketResponse rawData){
            var dungeon = DungeonManager.activeDungeons[Convert.ToUInt64(rawData.data)];
            dungeon.StopDungeon();
        }
    }
}
