using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void UpdateOsuId(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        user.ids.osuId = rawData.data;
        DBUtils.UpdateUser(user);
        //OsuApi.SendMessageToUser(user.ids.osuId, "salut moi");
        //OsuApi.GetUserRecentScores("11753335", "osu");
        OsuApi.SendMessageToUser("11753335", "727 when you see it");
    }
    protected void UpdateTheme(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        user.theme = rawData.data;
        DBUtils.UpdateUser(user);
    }
}