using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void Pull(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);

        var pullData = JsonConvert.DeserializeObject<GachaPullClientRequest>(rawData.data);
        if(user.gacha_currency < Gacha.GetBannerCost(pullData.bannerId, pullData.pullAmount)){
            Send(ClientNotification.NotificationData("Pulling", "You don't have enough gacha currency!", 3));
            return;
        }
        var waifus = Gacha.Pull(pullData.bannerId, pullData.pullAmount);
        user.waifus.AddRange(waifus);
        DBUtils.UpdateUser(user);
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "pull results",
            data = JsonConvert.SerializeObject(waifus)
        }));
        Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "user",
            data = JsonConvert.SerializeObject(user)
        }));
        
    }
}