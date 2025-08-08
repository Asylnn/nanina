namespace Nanina.Discord
{
    public class TokenResponse 
    {
        public required string access_token; 
        public required string token_type; 
        public int expires_in; 
        public required string refresh_token; 
        public required string scope; 
    }
}


