namespace Nanina.Osu
{
    public class OAuthTokens 
    {
        public int expires_in;
        public long expiration_timestamp;
        public string access_token = "";
        public string token_type = "";
        public string refresh_token = "";
    }
}