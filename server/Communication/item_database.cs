using System.Collections;
using LiteDB;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        /*
            This is how the client send back the item database
        */
        private class ItemDBResponse
        {
            public List<Item> material;
            public List<Item> waifu_consumable;
            public List<Item> user_consumable;
            public List<Equipment> equipment;
        }

        /*
            If the user is an admin, then it sends the item database
        */
        protected void ProvideItemDatabase(string userId)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            /* Get item col to find all and send a websocket containing the item database
            */
            DBUtils.SendDatabaseToClient(ID, "item");
        }

        protected void ProvideEquipmentDatabase(string userId)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            /* Get item col to find all and send a websocket containing the item database
            */
            DBUtils.SendDatabaseToClient(ID, "equipment");
        }
        protected void UpdateItemDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);

            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(user.admin == false)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}

            var itemDBResponse = JsonConvert.DeserializeObject<ItemDBResponse>(rawData.data);
            IEnumerable<Item> items = itemDBResponse.user_consumable.Concat(itemDBResponse.material).Concat(itemDBResponse.waifu_consumable);
            File.WriteAllText("../save/item.json", JsonConvert.SerializeObject(items));
            Send(ClientNotification.NotificationData("admin", "updated the item database!", 0));
        }

        protected void UpdateEquipmentDatabase(ClientWebSocketResponse rawData)
        {

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);

            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(user.admin == false)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}

            var itemDBResponse = JsonConvert.DeserializeObject<ItemDBResponse>(rawData.data);
            File.WriteAllText("../save/equipment.json", JsonConvert.SerializeObject(itemDBResponse.equipment));
            Send(ClientNotification.NotificationData("admin", "updated the equipment database!", 0));
        }

        protected void ProvideSetDatabase(string userId)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            /* Get set col to find all and send a websocket containing the set database
            */
            DBUtils.SendDatabaseToClient(ID, "set");
        }
        protected void UpdateSetDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.admin == false)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}
            
            /* Get set col to delete all then add all back*/
            File.WriteAllText("../save/set.json", rawData.data);
            Send(ClientNotification.NotificationData("admin", "updated the set database!", 0));
        }

        protected void GetLevelRewards(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user level rewards",
                data = JsonConvert.SerializeObject(Global.userLevelRewards) 
            }));
            
        }
    }
}
