using Nanina.Communication;
using Nanina.Database;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Dungeon
{
    public static class DungeonManager {
        public static readonly Dictionary<long, ActiveDungeon> activeDungeons = [];
        public static void InstantiateDungeon(Template dungeon, User user, List<Waifu> waifus, string sessionId, byte floor)
        {
            var instanceId = Utils.GetTimestamp();
            var activeDungeon = new ActiveDungeon(dungeon, user.Id, waifus, sessionId, instanceId, floor);
            activeDungeons.Add(instanceId, activeDungeon);
            
            user.dungeonInstanceId = activeDungeon.instanceId;

            ProvideDungeonToClient(activeDungeon);
        }

        public static void ProvideDungeonToClient(ActiveDungeon activeDungeon){

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
        
        public static void UpdateStatusOfDungeon(ActiveDungeon activeDungeon){
            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                var data = new
                {
                    lastLog = activeDungeon.log.Slice(activeDungeon.log.Count - 3, 3),
                    bossHealth = activeDungeon.health,
                };

                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.UpdateDungeon,
                    data = JsonConvert.SerializeObject(data)
                }), session.webSocketId);
            }
        }

        public static void FreeWaifus(ActiveDungeon activeDungeon)
        {

            var user = DBUtils.Get<User>(x => x.Id == activeDungeon.userId)!;
            //could also use user.waifuIdsInDungeon
            activeDungeon.waifus.ForEach(dungeonWaifu =>
            {
                user.waifus[dungeonWaifu.id].isDoingSomething = false;
            });


            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if (session == null) return;
            if (session.webSocketId != null)
            {
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.FreeWaifus,
                    data = JsonConvert.SerializeObject(user.waifuIdsInDungeon)
                }), session.webSocketId);
            }

            user.waifuIdsInDungeon = [];
            DBUtils.Update(user);
        }
        

        public static void SendLootToClient(ActiveDungeon activeDungeon, List<Loot> loot)
        {
            var session = DBUtils.Get<Session>(x => x.id == activeDungeon.sessionId);
            if(session == null) return;
            if(session.webSocketId != null)
            {
                
                Global.ws.WebSocketServices["/ws"].Sessions.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
                {
                    type = ServerResponseType.ProvideAndGiveLoot,
                    data = JsonConvert.SerializeObject(loot)
                }), session.webSocketId);
            }
        }
    }
}
