using Nanina.Database;
using Nanina.UserData;
using Nanina.UserData.WaifuData;
using Newtonsoft.Json;

namespace Nanina.Gacha
{
    public static class GachaManager {    
        public static readonly Banner[] banners = JsonConvert.DeserializeObject<Banner[]>(File.ReadAllText(Global.config.banners_storage_path));
        public static ushort GetBannerCost(string id, byte pullAmount)
        {
            var banner = banners.ToList().Find(banner => id == banner.id);
            return (ushort)(banner.pullCost*pullAmount);
        }
        public static bool BannerExists(string bannerId)
        {
            return banners.Any(x => x.id == bannerId);
        }
        public static List<Waifu>Pull(User user, string bannerId, ushort pullAmount){
            
            
            if(! user.pullBannerHistory.ContainsKey(bannerId)) {
                user.pullBannerHistory[bannerId] = new PullBannerHistory {
                pullHistory = [],
                pullBeforePity = pullAmount,
            };}

            var banner = banners.ToList().Find(banner => bannerId == banner.id);
            var weight = banner.twoStarsWeight + banner.rateUpThreeStarsWeight + banner.rateUpTwoStarsWeight + banner.threeStarsWeight;
            var pityWeight = banner.threeStarsWeight + banner.rateUpThreeStarsWeight;
            
            List<Waifu> waifus = [];
            for(var i = 0; i < pullAmount; i++){
                string[] waifuPool = [];
                Random rng = new Random();
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
                waifus.Add(DBUtils.GetWaifu(waifuId));
            }
            return waifus;
        }
    }

}