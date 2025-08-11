using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Dungeon
{
    public static class DungeonManager {
        public static readonly Dictionary<string, Template> dungeons = JsonConvert.DeserializeObject<Dictionary<string, Template>>(File.ReadAllText(Global.config.dungeon_storage_path.ToString()))!;
        public static readonly Dictionary<long, ActiveDungeon> activeDungeons = [];
        public static void InstantiateDungeon(Template dungeon, User user, List<Waifu> waifus, string sessionId, byte floor)
        {
            var instanceId = Utils.GetTimestamp();
            var activeDungeon = new ActiveDungeon(dungeon, user.Id, waifus, sessionId, instanceId, floor);
            activeDungeons.Add(instanceId, activeDungeon);
            
            user.dungeonInstanceId = activeDungeon.instanceId;

            UpdateDungeonOfClient(activeDungeon);
        }

        public static void UpdateDungeonOfClient(ActiveDungeon activeDungeon){

            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.ProvideActiveDungeon,
                    data = activeDungeon.ToString()
                }), session.webSocketId);
            }
        }

        public static void FreeWaifus(ActiveDungeon activeDungeon){

            var user = DBUtils.Get<User>(x => x.Id == activeDungeon.userId)!;
            activeDungeon.waifus.ForEach(dungeonWaifu =>
            {
                user.waifus[dungeonWaifu.id].isDoingSomething = false;
            });
            user.waifuIdsInDungeon = [];
            DBUtils.Update(user);

            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.ProvideUser,
                    data = JsonConvert.SerializeObject(user)
                }), session.webSocketId);
            }
        }
        

        public static void SendLootToClient(ActiveDungeon activeDungeon, List<Loot> loot)
        {
            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.ProvideLoot,
                    data = JsonConvert.SerializeObject(loot)
                }), session.webSocketId);
            }
        }
    }
}
