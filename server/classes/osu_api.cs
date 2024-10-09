using System.Net.Http;
using RestSharp;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

public class OsuOAuthTokens {
    public int expires_in;
    public string access_token;
    public string token_type;
}

public class OsuScore {

}

public static class OsuApi {
    public static RestClient client = new RestClient(Environment.GetEnvironmentVariable("OSU_API_URL"));
    public static OsuOAuthTokens tokens = new OsuOAuthTokens();

    public static async void RefreshTokens(){
        var request = new RestRequest("oauth/token", Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Accept", "application/json");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"));
        request.AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"));
        request.AddParameter("scope","public");
        var response = await client.ExecutePostAsync(request);
        Console.WriteLine(response.Content);
        tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OsuOAuthTokens>(response.Content);
    }

    public static async void GetUserRecentScores(string id, string mode){
        var request = new RestRequest($"users/{{id}}/scores/recent", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer " + tokens.access_token);
        request.AddParameter("user",id);
        request.AddParameter("type","recent");
        request.AddQueryParameter("limit",1);
        request.AddQueryParameter("mode",mode);
        var response = await client.ExecuteGetAsync(request);
        Console.WriteLine(response.Content);
    }
}