using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using Newtonsoft.Json;
using WebSocketSharp.Server;

public static class DungeonManager {
    public static readonly DungeonTemplate[] dungeons = JsonConvert.DeserializeObject<DungeonTemplate[]>(File.ReadAllText(Global.config.dungeon_storage_path.ToString())); //Doesn't work with ToString ???
    private static readonly Dictionary<ulong, ActiveDungeon> activeDungeons = [];
    private static ulong counter = 0;
    //Should be mutexed before release
    public static ActiveDungeon InstantiateDungeon(DungeonTemplate dungeon, User user, List<Waifu> waifus, string wsId, WebSocketSessionManager WSSM){
        var activeDungeon = new ActiveDungeon(dungeon, user, waifus, wsId, WSSM);
        activeDungeons.Add(counter, activeDungeon);
        counter++;
        return activeDungeon;
    }

    //While it's necessary to put public here, I don't feel like it's a great idea... Maybe I should try to find a better more conventionnal way to send a packet.
    public static void UpdateDungeonOfClient(ActiveDungeon activeDungeon){
        activeDungeon.WSSession.SendTo(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "dungeon",
            data = activeDungeon.ToString()
        }), activeDungeon.webSocketId);
    }
}