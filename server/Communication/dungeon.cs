using Nanina.Database;
using Nanina.Dungeon;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void StartDungeon(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.GetSession(rawData.sessionId);
            if(user == null) {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}
            var dungeon = DungeonManager.dungeons.Where(dungeon => dungeon.id == rawData.data).First();
            if(dungeon == null) {Send(ClientNotification.NotificationData("Dungeon", "The dungeon you tried to start doesn't exist!", 1)); return ;}
            DungeonManager.InstantiateDungeon(dungeon, user, user.waifus, session.id);
        }

        protected void StopDungeon(ClientWebSocketResponse rawData){
            var dungeon = DungeonManager.activeDungeons[(ulong)Convert.ToInt64(rawData.data)];
            dungeon.StopDungeon();
        }
    }
}
