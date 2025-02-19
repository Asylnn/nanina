using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.UserData.WaifuData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void ProvideWaifuDatabase(string userId)
        {
            var user = DBUtils.GetUser(userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            
            var waifuCol = DBUtils.GetCollection<Waifu>();
            var data = JsonConvert.SerializeObject(waifuCol.FindAll());
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "waifu db",
                data = data
            }));
        }
        protected void UpdateWaifuDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}

            var waifus = JsonConvert.DeserializeObject<Waifu[]>(rawData.data);
            var waifuCol = DBUtils.GetCollection<Waifu>();
            waifuCol.DeleteAll();
            foreach (var waifu in waifus) 
            {
                waifuCol.Insert(waifu);
                waifuCol.EnsureIndex(x => x.id, true);
            }
            Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
            
        }
    }
}
