namespace Nanina.Gacha
{
    public class Banner {
        public required string bannerName;
        public required string bannerDescription;
        public required string id;
        public ushort pityAmount;
        public ushort pullCost;
        public uint twoStarsWeight;
        public uint threeStarsWeight;
        public uint rateUpTwoStarsWeight;
        public uint rateUpThreeStarsWeight;
        public required string[] twoStarsPoolId;
        public required string[] threeStarsPoolId;
        public required string[] rateUpTwoStarsPoolId;
        public required string[] rateUpThreeStarsPoolId;
    }
}
