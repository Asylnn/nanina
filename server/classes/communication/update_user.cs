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
    }
    protected void UpdateTheme(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        user.theme = rawData.data;
        DBUtils.UpdateUser(user);
    }
}