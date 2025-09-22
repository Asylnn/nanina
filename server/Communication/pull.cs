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
        public class PullRequest
        {
            public string? bannerId {get; set;}
            public byte pullAmount {get; set;}
        }
        protected void Pull(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user is null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return ;}
            if(Utils.TryDeserialize<PullRequest>(rawData.data, out var pullData) == false)
                {Send(ClientNotification.NotificationData("Pulling", "pull data is null!", 1)); return;}
            if(pullData.bannerId is null)
                {Send(ClientNotification.NotificationData("Pulling", "The banner you tried to pull on doesn't exists!", 1)); return;}
            if(Global.banners.TryGet(pullData.bannerId, out var banner) == false)
                {Send(ClientNotification.NotificationData("Pulling", "The banner you tried to pull on doesn't exists!", 1)); return;}
            var bannerCost = banner.pullCost * pullData.pullAmount;
            if(user.gacha_currency < bannerCost)
                {Send(ClientNotification.NotificationData("Pulling", "You don't have enough gacha currency!", 3));return;}
            if(pullData.pullAmount != 1 && pullData.pullAmount != 10)
                {Send(ClientNotification.NotificationData("Pulling", "You can't pull a different amount of 1 or 10 times", 1)); return;}


            user.gacha_currency -= bannerCost;

            if(user.pullBannerHistory.ContainsKey(banner.id) == false)
            {
                user.pullBannerHistory[banner.id] = new()
                {
                    pullBeforePity = banner.pityAmount,
                };
            }

            var pullBannerHistory = user.pullBannerHistory[banner.id];
            Utils.ConsoleObject(banner);
            var weight = banner.standardPool.Aggregate(0d, (weight, pool) => weight + pool.weight);
            var pityWeight = banner.pityPool.Aggregate(0d, (weight, pool) => weight + pool.weight);
            List<string> waifusId = [];

            for(var i = 0; i < pullData.pullAmount; i++){
                List<BannerPoolElement> pool = [];
                Random rng = new ();
                var randomWeight = user.pullBannerHistory[banner.id].pullBeforePity == 0 ? pityWeight*rng.NextDouble() : rng.NextDouble()*weight;

                if(pullBannerHistory.pullBeforePity != 0)
                {
                    pullBannerHistory.pullBeforePity -= 1;
                    pool = banner.standardPool;
                }
                else
                {
                    pullBannerHistory.pullBeforePity -= banner.pityAmount;
                    pool = banner.pityPool;
                }
                waifusId.Add(pool.First((poolElement) =>
                    {
                        randomWeight -= poolElement.weight;
                        return randomWeight <= 0;
                    }).waifuId);
            }


            pullBannerHistory.pullHistory.AddRange(waifusId);
            pullBannerHistory.pullBeforePity -= pullData.pullAmount;

            /*A deep copy should not be required since it will no longer exist in memory after it goes into the DB,
                but in case any properties need to be modified sometime in the future, then there won't be some difficult to find a bug*/
            var waifus = waifusId.ConvertAll(waifuId => Utils.DeepCopyReflection(Global.waifus[waifuId])!);

            var alreadyOwnedWaifus = waifus.Where(pulledWaifu => user.waifus.ContainsKey(pulledWaifu.id));
            var notOwnedWaifus = waifus.Where(pulledWaifu => !user.waifus.ContainsKey(pulledWaifu.id));
            Utils.ConsoleObject(alreadyOwnedWaifus);
            Utils.ConsoleObject(notOwnedWaifus);

            List<Waifu> aquiredWaifus = [];
            foreach(var waifu in notOwnedWaifus)
            {
                Console.WriteLine(!aquiredWaifus.Any(aquiredWaifu => waifu.id == aquiredWaifu.id));
                if (!aquiredWaifus.Any(aquiredWaifu => waifu.id == aquiredWaifu.id))
                    aquiredWaifus.Add(waifu);
                else
                    alreadyOwnedWaifus = alreadyOwnedWaifus.Append(waifu);
            }

            Utils.ConsoleObject(alreadyOwnedWaifus);
            Utils.ConsoleObject(aquiredWaifus);
            /*get item db to find one item?*/
            var baseItem = Global.items[10_000]; //Item id for waifu essence
            List<Item> aquiredItems = [];
            foreach(var waifu in alreadyOwnedWaifus){
                var item = Utils.DeepCopyReflection(baseItem)!;
                item.id += Convert.ToInt16(waifu.id);
                aquiredItems.Add(item);
                user.inventory.AddItem(item);
            }

            foreach(var waifu in aquiredWaifus)
                user.waifus[waifu.id] = waifu;

            DBUtils.Update(user);
            var data = new
            {
                waifus=JsonConvert.SerializeObject(waifus),
                items=JsonConvert.SerializeObject(aquiredItems),
            };
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvidePullResults,
                data = JsonConvert.SerializeObject(data),
            }));
        }
    }
}