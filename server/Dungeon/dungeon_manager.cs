using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Dungeon
{
    public static class DungeonManager {
        public static readonly Template[] dungeons = JsonConvert.DeserializeObject<Template[]>(File.ReadAllText(Global.config.dungeon_storage_path.ToString())); //Doesn't work with ToString ???
        public static readonly Dictionary<ulong, ActiveDungeon> activeDungeons = [];
        private static ulong counter = 0;
        //Should be mutexed before release
        public static void InstantiateDungeon(Template dungeon, User user, List<Waifu> waifus, string sessionId)
        {
            var activeDungeon = new ActiveDungeon(dungeon, user, waifus, sessionId, counter);
            activeDungeons.Add(counter, activeDungeon);
            UpdateDungeonOfClient(activeDungeon);
        }

        public static void UpdateDungeonOfClient(ActiveDungeon activeDungeon){

            var session = DBUtils.GetSession(activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = "get active dungeon",
                    data = activeDungeon.ToString()
                }), session.webSocketId);
            }
        }
    }
}
