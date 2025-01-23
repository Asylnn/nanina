using RestSharp;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
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
}



public static class OsuApi {
    public static RestClient client = new RestClient(Environment.GetEnvironmentVariable("OSU_API_URL"));
    public static OsuOAuthTokens tokens = new OsuOAuthTokens();

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
        //Console.WriteLine("yoken 0 " + response.Content);
        tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OsuOAuthTokens>(response.Content);

        File.WriteAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH"), response.Content);
    }

    public static async Task<OsuScoreExtended[]> GetUserRecentScores(string id, string mode){
        var request = new RestRequest($"users/{id}/scores/recent", Method.Get);
        AddDefaultHeader(request);
        request.AddQueryParameter("limit","11");
        request.AddQueryParameter("mode",mode);
        var response = await client.ExecuteGetAsync(request);
        Console.WriteLine("id : " + id + " mode : "  + mode);

        Console.WriteLine("response content "+ response.Content);
        Console.WriteLine("response status code "+ response.StatusCode);
        
        if(response.IsSuccessStatusCode)
        {
            //try { //Temporary fix, seems like empty arrays are crashing Newtonsoft
            Console.WriteLine("ehe");
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject<OsuScoreExtended[]>(response.Content,  new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            })));
            Console.WriteLine("ehe2");

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
            Console.WriteLine("Is not a Success");
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
}