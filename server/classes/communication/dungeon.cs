using Newtonsoft.Json;
using WebSocketSharp.Server;

partial class WS : WebSocketBehavior
{
    protected void StartDungeon(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        var dungeon = DungeonManager.dungeons.Where(dungeon => dungeon.id == rawData.data).First();
        if(dungeon == null) {Send(ClientNotification.NotificationData("Dungeon", "The dungeon you tried to start doesn't exist!", 1)); return ;}
        var activeDungeon = DungeonManager.InstantiateDungeon(dungeon, user, user.waifus);
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "dungeon",
            data = JsonConvert.SerializeObject(activeDungeon)
        }));
    }

    //While it's necessary to put public here, I don't feel like it's a great idea... Maybe I should try to find a better more conventionnal way to send a packet.
    public void UpdateDungeonOfClient(ActiveDungeon activeDungeon){
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "dungeon",
            data = JsonConvert.SerializeObject(activeDungeon)
        }));
    }
}