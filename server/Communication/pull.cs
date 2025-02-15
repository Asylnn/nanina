using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.Gacha;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void Pull(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}

            var pullData = JsonConvert.DeserializeObject<PullRequest>(rawData.data);
            if(pullData == null)
                {Send(ClientNotification.NotificationData("Pulling", "The banner you tried to pull on doesn't exists!", 1)); return;}
            if(user.gacha_currency < GachaManager.GetBannerCost(pullData.bannerId, pullData.pullAmount))
                {Send(ClientNotification.NotificationData("Pulling", "You don't have enough gacha currency!", 3));return;}
            if(!GachaManager.BannerExists(pullData.bannerId))
                {Send(ClientNotification.NotificationData("Pulling", "The banner you tried to pull on doesn't exists!", 1));return;}
            if(pullData.pullAmount != 1 && pullData.pullAmount != 10)
                {Send(ClientNotification.NotificationData("Pulling", "You can't pull a different amount of 1 or 10 times", 1)); return;}
            
                
            
            user.gacha_currency -= GachaManager.GetBannerCost(pullData.bannerId, pullData.pullAmount);
            var waifus = GachaManager.Pull(user, pullData.bannerId, pullData.pullAmount);
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
}
