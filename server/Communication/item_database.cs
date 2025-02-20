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
        protected void ProvideItemDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            /* Get item col to find all and send a websocket containing the item database
            
            */
            Console.WriteLine("je rentre dans send db client item");
            DBUtils.SendDatabaseToClient<Item>(ID);
        }
        protected void UpdateItemDatabase(ClientWebSocketResponse rawData)
        {

            var user = DBUtils.GetUser(rawData.userId);

            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(user.admin == false)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}

            var items = JsonConvert.DeserializeObject<ItemDBResponse>(rawData.data);
            
            /* Get item col to delete all all then add all back*/
            var itemCol = DBUtils.GetCollection<Item>();


            /* Get equip col to help insert equipments back in the db*/
            var equipmentCol = DBUtils.GetCollection<Equipment>();
            itemCol.DeleteAll();
            foreach (var item in items.material) {
                InsertItem(item, itemCol);
            }
            foreach (var item in items.waifu_consumable) {
                InsertItem(item, itemCol);
            }
            foreach (var item in items.user_consumable) {
                InsertItem(item, itemCol);
            }
            foreach (var item in items.equipment) {
                InsertEquipment(item, itemCol, equipmentCol);
            }
            Send(ClientNotification.NotificationData("admin", "updated the item database!", 0));
            
        }

        protected void ProvideSetDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action without being connected!", 1)); return ;}
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            /* Get set col to find all and send a websocket containing the set database
            */
            DBUtils.SendDatabaseToClient<Set>(ID);
        }
        protected void UpdateSetDatabase(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(user.admin == false)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) 
                {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}
            
            /* Get set col to delete all then add all back*/
            var sets = JsonConvert.DeserializeObject<Set[]>(rawData.data); 
            DBUtils.Rebuild(sets);
            
            Send(ClientNotification.NotificationData("admin", "updated the set database!", 0));
            
        }

        public static void InsertEquipment(Equipment item, ILiteCollection<Item> itemCol, ILiteCollection<Equipment> col){
            InsertItem(item, itemCol);
            col.EnsureIndex(x => x.setId, false);
        }
        public static void InsertItem(Item item, ILiteCollection<Item> col){
            col.Insert(item);
            col.EnsureIndex(x => x.id, true);
            col.EnsureIndex(x => x.type, false);
        }
    }
}
