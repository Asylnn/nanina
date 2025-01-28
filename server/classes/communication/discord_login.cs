using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected async void DiscordLogin(ClientWebSocketResponse rawData){
        Console.WriteLine("code : " + rawData.data);
        Console.WriteLine("url : " + Environment.GetEnvironmentVariable("DISCORD_API_URL") + "token");
        var base64code = $"{Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")}:{Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")}";

        var client = new RestClient(Environment.GetEnvironmentVariable("DISCORD_API_URL"));
        var request_access_token = new RestRequest("oauth2/token", Method.Post);
        request_access_token.AddHeader("Authorization", $"Basic {System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code))}");
        //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request_access_token.AddParameter("grant_type", "authorization_code");
        request_access_token.AddParameter("code", rawData.data);
        request_access_token.AddParameter("redirect_uri", Environment.GetEnvironmentVariable("DISCORD_REDIRECT_URI"));

        var response_access_token = await client.ExecutePostAsync(request_access_token);
        if(response_access_token.StatusCode != System.Net.HttpStatusCode.OK) {Console.WriteLine("Status code: " + response_access_token.StatusCode + " et " + response_access_token.Content);return;}
        var discordTokenResponse = JsonConvert.DeserializeObject<DiscordTokenResponse>(response_access_token.Content);

        Console.WriteLine("response access token data : " + response_access_token.Content);

        var request_user_information = new RestRequest("users/@me", Method.Get);
        request_user_information.AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");
        var response_user_information = await client.ExecuteGetAsync(request_user_information);
        if(response_user_information.StatusCode != System.Net.HttpStatusCode.OK) {return;}

        Console.WriteLine("response user information data : " + response_user_information.Content);
        var discordUserInformationResponse = JsonConvert.DeserializeObject<DiscordUserInformationResponse>(response_user_information.Content);

        using(var db = new LiteDatabase($@"{Environment.GetEnvironmentVariable("DATABASE_PATH")}")){
            var user_col = db.GetCollection<User>("userdb");
            //We have to unpoco the user for using the constructor
            User user;

            var list = user_col.Find(x => x.ids.discordId == discordUserInformationResponse.id);
            if (list.Count() == 0){

                var ids = new Ids() {discordId = discordUserInformationResponse.id};
                ids.discordId = discordUserInformationResponse.id;
                user = new (discordUserInformationResponse.global_name, ids);
                user.locale = discordUserInformationResponse.locale;
                
                Console.WriteLine("Inserted new user! Id : " + user.Id);
                var tokens = new Tokens(){
                    discord_access_token = discordTokenResponse.access_token,
                    discord_refresh_token = discordTokenResponse.refresh_token
                };
                user.tokens = tokens;
                user_col.Insert(user);
                user_col.EnsureIndex(x => x.ids.discordId, true);
                user_col.EnsureIndex(x => x.Id, true);
            }

            else if(list.Count() == 1){
                user = list.First();
                var tokens = new Tokens(){
                    discord_access_token = discordTokenResponse.access_token,
                    discord_refresh_token = discordTokenResponse.refresh_token
                };
                user.tokens = tokens;
                
                user_col.Update(user);

            }
            else {
                user = list.First();
                Console.Error.WriteLine("There is more than two people in the user database with the same discord user id! The id is : " + discordUserInformationResponse.id);
            }
            Send(JsonConvert.SerializeObject(Communication.UpdateSessionId(user.Id, true)));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                type = "user",
                data = JsonConvert.SerializeObject(user)
            }));
        }
    }
}