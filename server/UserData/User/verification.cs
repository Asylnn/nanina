namespace Nanina.UserData
{
    public class Verification 
    {
        public long osuVerificationCodeTimestamp { get; set; } = 0;
        public bool isOsuIdVerified { get; set; } = false;
        public string? osuVerificationCode { get; set; }
        public bool isMaimaiTokenVerified { get; set; } = false;
    }
}