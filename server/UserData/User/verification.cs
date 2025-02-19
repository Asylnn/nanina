namespace Nanina.UserData
{
    public class Verification 
    {
        public ulong osuVerificationCodeTimestamp { get; set; }
        public bool isOsuIdVerified { get; set; }
        public string osuVerificationCode { get; set; }
        public bool isMaimaiTokenVerified { get; set; }
    }
}