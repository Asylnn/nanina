using System.Globalization;
using System.Runtime.CompilerServices;

public class DiscordTokenResponse {
    public string access_token; 
    public string token_type; 
    public int expires_in; 
    public string refresh_token; 
    public string scope; 
}

public class DiscordUserInformationResponse {
    public string id;
    public string username;
    public string avatar;
    public string discriminator;
    public int public_flags;
    public int flags;
    public object banner;
    public int accent_color;
    public string global_name;
    public object avatar_decoration_box;
    public string banner_color;
    public object clan;
    public bool mfa_enabled;
    public string locale;
    public int premium_type; 
}