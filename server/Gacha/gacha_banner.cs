using Nanina.Database;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.Gacha
{
    public static class GachaManager {    
        public static ushort GetBannerCost(string id, byte pullAmount)
        {
            var banner = Global.banners.ToList().Find(banner => id == banner.id)!;
            return (ushort)(banner.pullCost*pullAmount);
        }
        public static bool BannerExists(string bannerId)
        {
            return Global.banners.Any(x => x.id == bannerId);
        }
        public static List<Waifu>Pull(User user, string bannerId, ushort pullAmount){
            
            var banner = Global.banners.ToList().Find(banner => bannerId == banner.id)!;
            
            if(! user.pullBannerHistory.ContainsKey(bannerId)) {
                user.pullBannerHistory[bannerId] = new PullBannerHistory {
                pullHistory = [],
                pullBeforePity = pullAmount,
            };}

            
            var weight = banner.twoStarsWeight + banner.rateUpThreeStarsWeight + banner.rateUpTwoStarsWeight + banner.threeStarsWeight;
            var pityWeight = banner.threeStarsWeight + banner.rateUpThreeStarsWeight;
            
            List<Waifu> waifus = [];
            for(var i = 0; i < pullAmount; i++){
                string[] waifuPool = [];
                Random rng = new ();
                var random = user.pullBannerHistory[bannerId].pullBeforePity == 0 ? weight - pityWeight + rng.NextDouble()*pityWeight : rng.NextDouble()*weight;
                if(random < banner.twoStarsWeight){    
                    waifuPool = banner.twoStarsPoolId;
                    user.pullBannerHistory[bannerId].pullBeforePity -= 1;
                }
                else if(random < banner.rateUpTwoStarsWeight + banner.twoStarsWeight){
                    waifuPool = banner.rateUpTwoStarsPoolId;
                    user.pullBannerHistory[bannerId].pullBeforePity -= 1;
                }
                else if(random < banner.threeStarsWeight + banner.rateUpTwoStarsWeight + banner.twoStarsWeight){
                    waifuPool = banner.threeStarsPoolId;
                    user.pullBannerHistory[bannerId].pullBeforePity = banner.pityAmount;
                }
                else {
                    waifuPool = banner.rateUpThreeStarsPoolId;
                    user.pullBannerHistory[bannerId].pullBeforePity = banner.pityAmount;

                }
                
                
                var waifuId = waifuPool.RandomElement();
                user.pullBannerHistory[bannerId].pullHistory.Add(waifuId);
                /*A deep copy should not be required since it will no longer exist in memory after it goes into the DB, 
                but in case any properties need to be modified sometime in the future, then there won't be some difficult to find bug*/
                waifus.Add(Utils.DeepCopyReflection(Global.waifus.Find(x => x.id == waifuId))!);
            }
            return waifus;
        }
    }

}