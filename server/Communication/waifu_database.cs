using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.UserData.WaifuData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void UpdateWaifuDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}

            //var waifus = JsonConvert.DeserializeObject<Waifu[]>(rawData.data);
            //Console.WriteLine("waifus: " + waifus);
            /*get waifu col to delete all and insert them back*/
            //BUtils.Rebuild<Waifu>(waifus);
            File.WriteAllText("../save/waifu.json", rawData.data);
            Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
            
        }
    }
}
