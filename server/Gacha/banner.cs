namespace Nanina.Gacha
{
    public class BannerPoolElement
    {
        public double weight;
        public required string waifuId;
    }
    public class Banner {
        public required string id;
        public short pityAmount;
        public short pullCost;
        public required List<BannerPoolElement> standardPool;
        public required List<BannerPoolElement> pityPool;
    }
}
