using RestSharp;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
public enum Mod {
    NC,
    HD,
    HR,
    DT,
    EZ,
    FL,
    RX,
    AP,
    HT,
}
public class OsuOAuthTokens {
    public int expires_in;
    public string access_token;
    public string token_type;
    public string refresh_token;
}



public static class OsuApi {
    public static RestClient client = new RestClient(Environment.GetEnvironmentVariable("OSU_API_URL"));
    public static OsuOAuthTokens tokens = new OsuOAuthTokens();
    public static OsuOAuthTokens chat_tokens = new OsuOAuthTokens();


    public static string Linkify(OsuBeatmap map){
        return $"{Environment.GetEnvironmentVariable("OSU_BEATMAP_URL")}{map.beatmapset_id}#{map.mode}/{map.id}";
    }
    public static void AddDefaultHeader(RestRequest request){
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {tokens.access_token}");
    }
    public static async void RefreshTokens(){
        var request = new RestRequest(Environment.GetEnvironmentVariable("OSU_OAUTH_URL"), Method.Post);
        //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Accept", "application/json");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"));
        request.AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"));
        request.AddParameter("scope","public");
        var response = await new RestClient().ExecutePostAsync(request);
        Console.WriteLine("updated osu tokens");
        tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OsuOAuthTokens>(response.Content);

        File.WriteAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH"), response.Content);
    }

    public static async Task<OsuScoreExtended[]> GetUserRecentScores(string id, string mode){
        var request = new RestRequest($"users/{id}/scores/recent", Method.Get);
        AddDefaultHeader(request);
        request.AddQueryParameter("limit","11");
        request.AddQueryParameter("mode",mode);
        var response = await client.ExecuteGetAsync(request);

        Console.WriteLine("get recent scores from id : " + id + " mode : "  + mode);
        Console.WriteLine("response status code "+ response.StatusCode);
        
        if(response.IsSuccessStatusCode)
        {
            //try { //Temporary fix, seems like empty arrays are crashing Newtonsoft
            return Newtonsoft.Json.JsonConvert.DeserializeObject<OsuScoreExtended[]>(response.Content,  new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            
            //}
          //  catch {
                Console.WriteLine("Caught error, scores might be empty?");
                return [];
        //}
        }
        else 
        {
            Console.WriteLine("Couldn't get scores!");
            return [];
        }
    }

    public static int GetXP(OsuScoreExtended score){
        return (int) Math.Ceiling(score.accuracy*score.beatmap.hit_length*score.beatmap.difficulty_rating);
    }
    
    public static async Task<OsuBeatmap> GetBeatmapById(string beatmapId){
        var request = new RestRequest($"beatmaps/lookup", Method.Get);
        AddDefaultHeader(request);
        request.AddQueryParameter("id",beatmapId);

        
        var response = await client.ExecuteGetAsync(request);
        Console.WriteLine("zonse content "+ response.Content);
        Console.WriteLine("osu api response status code "+ response.StatusCode);

        if(response.IsSuccessStatusCode)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<OsuBeatmap>(response.Content,  new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
        else 
        {
            return null;
        }
    }
    public static async void SendMessageToUser(string osuUserId, string message){
        var request = new RestRequest($"chat/new", Method.Post);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {chat_tokens.access_token}");
        request.AddJsonBody(JsonConvert.SerializeObject(new {
            target_id=11753335,
            is_action=false,
            message="CA MARCHE !!! YES PUTAIN! 727 727 727 727 727 727"
        }));
        
        Console.WriteLine("osu id " + Convert.ToInt64(osuUserId));
        Console.WriteLine("osu id " + osuUserId);

        /*request.AddParameter("target_id", 2070907); //must be integer? YES
        request.AddParameter("message", "727");
        //request.AddParameter("type", "PM", , true);
        request.AddParameter("is_action", false );*/
        //Console.WriteLine(JsonConvert.SerializeObject(request));
        
        var response = await client.ExecutePostAsync(request);
        //var response2 = client.Execute(request);
        Console.WriteLine("zonse content "+ response.Content);
        Console.WriteLine("osu api response status code " + response.StatusCode);


    }

    public static async void AuthorizeSelf(string code){

        // for sending messages used for verifications, to get the code, check the following link :
        //https://osu.ppy.sh/oauth/authorize?client_id=422727&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=chat.write
        //replace client_id with your osu client id, same with the redicted_uri
        
        var request = new RestRequest("https://osu.ppy.sh/oauth/token", Method.Post);

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Accept", "application/json");

        
        request.AddParameter("grant_type", "authorization_code");
        request.AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"));
        request.AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"));
        request.AddParameter("redirect_uri", Environment.GetEnvironmentVariable("OSU_REDIRECT_URI"));
        request.AddParameter("code", code);


        var response = await new RestClient().ExecutePostAsync(request);

        Console.WriteLine("updated osu chat tokens");
        Console.WriteLine("osu api response status code " + response.StatusCode);
        File.WriteAllText(Environment.GetEnvironmentVariable("OSU_API_CHAT_TOKEN_STORAGE_PATH"), response.Content);


    }
}