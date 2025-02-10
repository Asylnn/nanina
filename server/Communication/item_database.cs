using LiteDB;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        private class ItemDBResponse
        {
            public List<Material> material;
            public List<WaifuConsumable> waifu_consumable;
            public List<UserConsumable> user_consumable;
            public List<Equipment> equipment;

        }
        protected void RequestItemDatabase(ClientWebSocketResponse rawData){
            if(!DBUtils.GetUser(rawData.id).admin){Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                
                var itemCol = db.GetCollection<Item>("itemdb");
                var data = JsonConvert.SerializeObject(itemCol.FindAll());

                var response = new ServerWebSocketResponse
                {
                    type = "item db",
                    data = data
                };
                Send(JsonConvert.SerializeObject(response));
            }
        }
        protected void UpdateItemDatabase(ClientWebSocketResponse rawData){

            

            if(DBUtils.GetUser(rawData.id).admin == false){Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
            if(!Global.config.dev) {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}
            Console.WriteLine("this is the items before deserializations: ");
            Console.WriteLine(rawData.data);

            var items = JsonConvert.DeserializeObject<ItemDBResponse>(rawData.data);
            Console.WriteLine("this is the items : ");
            Console.WriteLine(JsonConvert.SerializeObject(items));
            using(var db = new LiteDatabase($@"{Global.config.database_path}")){
                var itemCol = db.GetCollection<Item>("itemdb");
                var equipmentCol = db.GetCollection<Equipment>("itemdb");
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
                Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
            }
        }

        public static void InsertEquipment(Equipment item, ILiteCollection<Item> itemCol, ILiteCollection<Equipment> col){
            InsertItem(item, itemCol);
            col.EnsureIndex(x => x.setId, false);
        }
        public static void InsertItem(Item item, ILiteCollection<Item> col){
            col.Insert(item);
            col.EnsureIndex(x => x.id, true);
        }
    }
}
