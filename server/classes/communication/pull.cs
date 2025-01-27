using WebSocketSharp.Server;
using Newtonsoft.Json;

partial class WS : WebSocketBehavior
{
    protected void Pull(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);

        var pullData = JsonConvert.DeserializeObject<GachaPullClientRequest>(rawData.data);
        if(!Gacha.BannerExists(pullData.bannerId)){
            Send(ClientNotification.NotificationData("Pulling", "The banner you tried to pull on doesn't exists!", 1));
            return;
        }
        if(user.gacha_currency < Gacha.GetBannerCost(pullData.bannerId, pullData.pullAmount)){
            Send(ClientNotification.NotificationData("Pulling", "You don't have enough gacha currency!", 3));
            return;
        }
        if(pullData.pullAmount == 1 && pullData.pullAmount == 10){
            Send(ClientNotification.NotificationData("Pulling", "You can't pull a different amount of 1 or 10 times", 1));
            return;
        }
        user.gacha_currency -= Gacha.GetBannerCost(pullData.bannerId, pullData.pullAmount);
        var waifus = Gacha.Pull(user, pullData.bannerId, pullData.pullAmount);
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