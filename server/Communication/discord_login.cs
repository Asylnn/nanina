using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;
using Nanina.Database;
using Nanina.UserData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        /*
            This method is for authentificating the session when connecting with Discord (using OAuth)
        */
        protected async void DiscordLogin(ClientWebSocketResponse rawData)
        {
            
            Console.WriteLine("code : " + rawData.data);
            var base64code = $"{Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")}:{Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")}";

            var client = new RestClient(Global.config.discord_api_url); //Used for making requests to the API
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            //Discord asks for which URI you are redirected to, In case of dev, this is localhost:XXXX
            var redirect_uri = Global.config.dev ? Environment.GetEnvironmentVariable("DEV_DISCORD_REDIRECT_URI") : Environment.GetEnvironmentVariable("PROD_DISCORD_REDIRECT_URI") ;

            var request_access_token = new RestRequest("oauth2/token", Method.Post)
                .AddHeader("Authorization", $"Basic {System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(base64code))}")
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("code", rawData.data)
                .AddParameter("redirect_uri", redirect_uri);

            var response_access_token = await client.ExecutePostAsync(request_access_token);
            if(!response_access_token.IsSuccessStatusCode) 
                {Console.WriteLine("Status code: " + response_access_token.StatusCode + " et " + response_access_token.Content); return;}
            var discordTokenResponse = JsonConvert.DeserializeObject<Discord.TokenResponse>(response_access_token.Content);


            var request_user_information = new RestRequest("users/@me", Method.Get)
                .AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");

            var response_user_information = await client.ExecuteGetAsync(request_user_information);
            if(!response_access_token.IsSuccessStatusCode) return;

            var discordUserInformationResponse = JsonConvert.DeserializeObject<Discord.UserInformationResponse>(response_user_information.Content);

            var userCol = DBUtils.GetCollection<User>();
            User user;

            var list = userCol.Find(x => x.ids.discordId == discordUserInformationResponse.id);
            
            if (list.Count() == 0){

                var ids = new Ids() 
                {
                    discordId = discordUserInformationResponse.id
                };
                user = new (discordUserInformationResponse.global_name, ids);
                
                Console.WriteLine("Inserted new user! Id : " + user.Id);
                user.tokens = new Tokens()
                {
                    discord_access_token = discordTokenResponse.access_token,
                    discord_refresh_token = discordTokenResponse.refresh_token
                };
                userCol.Insert(user);
                userCol.EnsureIndex(x => x.ids.discordId, true);
                userCol.EnsureIndex(x => x.Id, true);
            }

            else if(list.Count() == 1){
                user = list.First();
                user.tokens = new Tokens(){
                    discord_access_token = discordTokenResponse.access_token,
                    discord_refresh_token = discordTokenResponse.refresh_token
                };
                
                userCol.Update(user);

            }
            else {
                user = list.First();
                Console.Error.WriteLine("There is more than two people in the user database with the same discord user id! The id is : " + discordUserInformationResponse.id);
            }
            var session = DBUtils.GetSession(rawData.sessionId);
            if(session == null) //Should not happen.
                session = Session.NewSession(this.ID);

            rawData.userId = user.Id;
            session.UpdateUserId(user.Id);            
            ProvideSessionToClient(rawData);
        }
    }
}
