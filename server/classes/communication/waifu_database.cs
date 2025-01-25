using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void RequestWaifuDatabase(ClientWebSocketResponse rawData){
        if(!DBUtils.GetUser(rawData.id).admin){Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var waifusDB = db.GetCollection<PocoWaifu>("waifudb").FindAll();
            Console.WriteLine(waifusDB);

            var data = JsonConvert.SerializeObject(waifusDB);
            Console.WriteLine(data);

            var response = new ServerWebSocketResponse
            {
                type = "waifu db",
                data = data
            };
            Send(JsonConvert.SerializeObject(response));
        }
    }
    protected void UpdateWaifuDatabase(ClientWebSocketResponse rawData){
        if(DBUtils.GetUser(rawData.id).admin == false){Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
        var waifus = JsonConvert.DeserializeObject<PocoWaifu[]>(rawData.data);
        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var waifuCol = db.GetCollection<PocoWaifu>("waifudb");
            waifuCol.DeleteAll();
            /*var new_waifus = waifus.Where(x => !waifuCol.Exists(y => y.id == x.id));
            var old_waifus = waifus.Where(x => waifuCol.Exists(y => y.id == x.id));
            Console.WriteLine(JsonConvert.SerializeObject(new_waifus));
            Console.WriteLine(JsonConvert.SerializeObject(old_waifus));*/
            foreach (var waifu in waifus) {
                waifuCol.Insert(waifu);
                waifuCol.EnsureIndex(x => x.id, true);
            }
            /*foreach (var waifu in old_waifus) {
                waifuCol.Update(waifu);
                //Update Many??
            }*/
            Send(ClientNotification.NotificationData("admin", "updated the waifu database!", 0));
        }
    }
}