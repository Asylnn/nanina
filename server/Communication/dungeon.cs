using Nanina.Database;
using Nanina.Dungeon;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void StartDungeon(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.id);
            var dungeon = DungeonManager.dungeons.Where(dungeon => dungeon.id == rawData.data).First();
            if(dungeon == null) {Send(ClientNotification.NotificationData("Dungeon", "The dungeon you tried to start doesn't exist!", 1)); return ;}
            DungeonManager.InstantiateDungeon(dungeon, user, user.waifus, ID, Sessions);
        }

        protected void StopDungeon(ClientWebSocketResponse rawData){
            var dungeon = DungeonManager.activeDungeons[(ulong)Convert.ToInt64(rawData.data)];
            dungeon.StopDungeon();
        }
    }
}
