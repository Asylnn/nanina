using LiteDB;
using Newtonsoft.Json;
using WebSocketSharp.Server;

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
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            
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
        if(Environment.GetEnvironmentVariable("DEV") != "true") {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}
        Console.WriteLine("this is the items before deserializations: ");
        Console.WriteLine(rawData.data);

        var items = JsonConvert.DeserializeObject<ItemDBResponse>(rawData.data);
        Console.WriteLine("this is the items : ");
        Console.WriteLine(JsonConvert.SerializeObject(items));
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var itemCol = db.GetCollection<Item>("itemdb");
            itemCol.DeleteAll();
            foreach (var item in items.material) {
                itemCol.Insert(item);
                itemCol.EnsureIndex(x => x.id, true);
            }
            foreach (var item in items.waifu_consumable) {
                itemCol.Insert(item);
                itemCol.EnsureIndex(x => x.id, true);
            }
            foreach (var item in items.user_consumable) {
                itemCol.Insert(item);
                itemCol.EnsureIndex(x => x.id, true);
            }
            foreach (var item in items.equipment) {
                itemCol.Insert(item);
                itemCol.EnsureIndex(x => x.id, true);
            }
            Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
        }
    }

    public static void InsertEquipment(Equipment item, ILiteCollection<Equipment> col){
        InsertItem(item, (ILiteCollection<Item>) col);
        col.EnsureIndex(x => x.setId, false);
    }
    public static void InsertItem(Item item, ILiteCollection<Item> col){
        col.Insert(item);
        col.EnsureIndex(x => x.id, true);
    }
}