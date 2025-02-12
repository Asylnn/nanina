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
        public static void InstantiateDungeon(Template dungeon, User user, List<Waifu> waifus, string sessionId){
            var activeDungeon = new ActiveDungeon(dungeon, user, waifus, sessionId, counter);
            activeDungeons.Add(counter, activeDungeon);
            counter++;
            UpdateDungeonOfClient(activeDungeon);
        }

        //While it's necessary to put public here, I don't feel like it's a great idea... Maybe I should try to find a better more conventionnal way to send a packet.
        public static void UpdateDungeonOfClient(ActiveDungeon activeDungeon){
            try 
            {
                var session = DBUtils.GetSession(activeDungeon.sessionId);
                Console.WriteLine(JsonConvert.SerializeObject(session));
                if(session.isConnectedToClient)
                {
                    Global.ws.WebSocketServices["/"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = "get active dungeon",
                        data = activeDungeon.ToString()
                    }), session.webSocketId);
                }
            }
            catch {}
        }
    }
}
