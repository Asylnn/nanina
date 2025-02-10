using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected void RequestWaifuDatabase(ClientWebSocketResponse rawData){
        if(!DBUtils.GetUser(rawData.id).admin){Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}
        using(var db = new LiteDatabase($@"{Global.config.database_path}")){
            var waifusDB = db.GetCollection<Waifu>("waifudb").FindAll();

            var data = JsonConvert.SerializeObject(waifusDB);

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
        if(!Global.config.dev) {Send(ClientNotification.NotificationData("admin", "The server isn't in developpement mode, you can't do this action", 0)); return;}
        var waifus = JsonConvert.DeserializeObject<Waifu[]>(rawData.data);
        using(var db = new LiteDatabase($@"{Global.config.database_path}")){
            var waifuCol = db.GetCollection<Waifu>("waifudb");
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