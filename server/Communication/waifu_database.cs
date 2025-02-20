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
            

            /*get waifu col to findall and send a websocket containing the whole waifu database*/
            DBUtils.SendDatabaseToClient<Waifu>(ID);
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
            Console.WriteLine("waifus: " + waifus);
            /*get waifu col to delete all and insert them back*/
            DBUtils.Rebuild<Waifu>(waifus);
            Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
            
        }
    }
}
