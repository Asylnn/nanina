using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.UserData.WaifuData;
using Nanina.UserData.ItemData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected void Pull(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
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
            Console.WriteLine("hey");
                
            user.gacha_currency -= GachaManager.GetBannerCost(pullData.bannerId, pullData.pullAmount);
            var waifus = GachaManager.Pull(user, pullData.bannerId, pullData.pullAmount);
            Console.WriteLine(JsonConvert.SerializeObject(waifus));
            Console.WriteLine(JsonConvert.SerializeObject(user.waifus));
            var alreadyOwnedWaifus = waifus.Where(pulledWaifu => user.waifus.Any(userWaifu => pulledWaifu.id == userWaifu.id));
            var notOwnedWaifus = waifus.Where(pulledWaifu => !user.waifus.Any(userWaifu => pulledWaifu.id == userWaifu.id));
            Console.WriteLine("fff0");
            Console.WriteLine(JsonConvert.SerializeObject(notOwnedWaifus));
            Console.WriteLine("fff1");
            Console.WriteLine(alreadyOwnedWaifus.Count());
            Console.WriteLine("fff2");
            Console.WriteLine(JsonConvert.SerializeObject(notOwnedWaifus));
            Console.WriteLine("hey2");
            List<Waifu> aquiredWaifus = [];
            foreach(var waifu in notOwnedWaifus){
                if(!aquiredWaifus.Any(aquiredWaifu => waifu.id == aquiredWaifu.id)){
                    aquiredWaifus.Add(waifu);
                }
                else 
                {
                    alreadyOwnedWaifus.Append(waifu);
                }
            }
            Console.WriteLine("hey3");

            /*get item db to find one item?*/
            var baseItem = Global.items.Find(item => item.id == 50_000); //Item id for waifu essence
            foreach(var waifu in alreadyOwnedWaifus){
                var item = baseItem.Clone() as Item;
                item.id += Convert.ToUInt16(waifu.id);
                user.inventory.AddMaterial(item);
            }

            Console.WriteLine("hey5");
            Console.WriteLine("hey");
            Console.WriteLine("hey");

            Console.WriteLine(JsonConvert.SerializeObject(aquiredWaifus));
            user.waifus.AddRange(aquiredWaifus);




            DBUtils.Update(user);
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
