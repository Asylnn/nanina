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
            
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session?.userId is not null)
            {
                var _user = DBUtils.Get<User>(x => x.Id == session.userId);
                if(_user is not null)
                {
                    ProvideSessionToClient(rawData); return;
                }
            }

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
            var discordTokenResponse = JsonConvert.DeserializeObject<Discord.TokenResponse>(response_access_token.Content!)!;


            var request_user_information = new RestRequest("users/@me", Method.Get)
                .AddHeader("Authorization", $"Bearer {discordTokenResponse.access_token}");

            var response_user_information = await client.ExecuteGetAsync(request_user_information);
            if(!response_access_token.IsSuccessStatusCode) return;

            var discordUserInformationResponse = JsonConvert.DeserializeObject<Discord.UserInformationResponse>(response_user_information.Content!)!;
            

            /*  Recup la collection d'objects Users, on find un user avec le bon id,
                Si on le trouve pas, on l'insert a la db
                Si on le trouve, on l'update
                Sinon, on prend le premier de la liste (contenant 2 ou plus users) et on met message d'erreur
            */

            var user = DBUtils.Get<User>(x => x.ids.discordId == discordUserInformationResponse.id);
            
            if (user is null){

                var ids = new Ids()
                {
                    discordId = discordUserInformationResponse.id
                };
                user = new()
                {
                    username = discordUserInformationResponse.global_name,
                    ids = ids,
                };

                Console.WriteLine("Inserted new user! Id : " + user.Id);
                user.tokens = new Tokens()
                {
                    discord_access_token = discordTokenResponse.access_token,
                    discord_refresh_token = discordTokenResponse.refresh_token
                };
                user.avatarPATH = $"{Global.config.discord_avatar_url}/{discordUserInformationResponse.id}/{discordUserInformationResponse.avatar}.webp";
                DBUtils.Insert(user);
            }

            else 
            { 
                user.tokens.discord_access_token = discordTokenResponse.access_token;
                user.tokens.discord_refresh_token = discordTokenResponse.refresh_token;
                user.avatarPATH = $"{Global.config.discord_avatar_url}/{discordUserInformationResponse.id}/{discordUserInformationResponse.avatar}.webp";
                DBUtils.Update(user);
            }
            session ??= Session.NewSession(ID); //Should not happen.

            rawData.userId = user.Id;
            session.UpdateUserId(user.Id);            
            ProvideSessionToClient(rawData);
        }
    }
}