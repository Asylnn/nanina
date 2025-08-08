namespace Nanina.Discord
{
    public class UserInformationResponse 
    {
        public required string id;
        public required string username;
        public required string avatar;
        public required string discriminator;
        public int public_flags;
        public int flags;
        public required object banner;
        public int accent_color;
        public required string global_name;
        public required object avatar_decoration_box;
        public required string banner_color;
        public required object clan;
        public bool mfa_enabled;
        public required string locale;
        public int premium_type; 
    }
}