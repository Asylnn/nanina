using RestSharp;
using Newtonsoft.Json;
public enum Mod {

}
public class OsuOAuthTokens {
    public int expires_in;
    public string access_token;
    public string token_type;
}



public static class OsuApi {
    public static RestClient client = new RestClient(Environment.GetEnvironmentVariable("OSU_API_URL"));
    public static OsuOAuthTokens tokens = new OsuOAuthTokens();

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
        Console.WriteLine("yoken -1 " + tokens.access_token);

        File.WriteAllText(Environment.GetEnvironmentVariable("OSU_API_TOKEN_STORAGE_PATH"), response.Content);
    }

    public static async void GetUserRecentScores(string id, string mode){
        var request = new RestRequest($"users/{id}/scores/recent", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {tokens.access_token}");
        request.AddQueryParameter("limit","11");
        request.AddQueryParameter("mode",mode);
        var response = await client.ExecuteGetAsync(request);
        Console.WriteLine("response content "+ response.Content);
        Console.WriteLine("response status code "+ response.StatusCode);
        var scores =  Newtonsoft.Json.JsonConvert.DeserializeObject<OsuScore[]>(response.Content,  new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        foreach (var score in scores) {
            var XP = score.accuracy*score.beatmap.hit_length*score.beatmap.difficulty_rating;
            Console.WriteLine($"You earned {XP} XP!");
        }
    }
}