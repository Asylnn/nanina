public class GachaPullClientRequest {
    public string bannerId {get; set;}
    public short pullAmount {get; set;}
}

public class Banner {
    public string bannerName;
    public string bannerDescription;
    public string bannerId;
    public short pityAmount;
    public int pullCost;
    public int twoStarsWeight;
    public int threeStarsWeight;
    public int rateUpTwoStarsWeight;
    public int rateUpThreeStarsWeight;
    public string[] twoStarsPollId;
    public string[] threeStarsPollId;
    public string[] rateUpTwoStarsPollId;
    public string[] rateUpThreeStarsPollId;
}

public class Gacha() {
    public static Banner[] banners;
    
    public static int GetBannerCost(string bannerId, short pullAmount){
        var banner = banners.ToList().Find(banner => bannerId == banner.bannerId);
        return banner.pullCost*pullAmount;
    }
    public static void LoadBanners(){
        banners = Newtonsoft.Json.JsonConvert.DeserializeObject<Banner[]>(File.ReadAllText(Environment.GetEnvironmentVariable("BANNER_STORAGE_PATH")));
    }
    public static List<PocoWaifu> Pull(string bannerId, short pullAmount){
        
        var banner = banners.ToList().Find(banner => bannerId == banner.bannerId);
        var weight = banner.twoStarsWeight + banner.rateUpThreeStarsWeight + banner.rateUpTwoStarsWeight + banner.threeStarsWeight;
        List<PocoWaifu> waifus = [];
        for(var i = 0; i < pullAmount; i++){
            string[] waifuPoll = [];
            Random rng = new Random();
            var random = rng.NextDouble()*weight;
            Console.WriteLine(random);
            Console.WriteLine(weight);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(waifuPoll));
            if(random < banner.twoStarsWeight){    
                waifuPoll = banner.twoStarsPollId;
            }
            else if(random < banner.rateUpTwoStarsWeight + banner.twoStarsWeight){
                waifuPoll = banner.rateUpTwoStarsPollId;
            }
            else if(random < banner.threeStarsWeight + banner.rateUpTwoStarsWeight + banner.twoStarsWeight){
                waifuPoll = banner.threeStarsPollId;
            }
            else {
                waifuPoll = banner.rateUpThreeStarsPollId;
            }
            
            
            var randdom = rng.Next(waifuPoll.Length);
            Console.WriteLine(randdom);
            Console.WriteLine(waifuPoll.Length);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(waifuPoll));
            var waifuId = waifuPoll.ElementAt(randdom);
            waifus.Add(DBUtils.GetWaifu(waifuId));
        }
        return waifus;
    }
}