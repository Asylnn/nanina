using RestSharp;
using Newtonsoft.Json;

namespace Nanina.Osu
{
    public static class Api {
        public static RestClient client = new RestClient(Global.config.osu_api_url);
        public static OAuthTokens tokens = new OAuthTokens();
        public static OAuthTokens chat_tokens = new OAuthTokens();


        /*public static string Linkify(Beatmap map){
            return $"{map.url}{map.beatmapset_id}#{map.mode}/{map.id}";
        }*/
        public static void AddDefaultHeader(RestRequest request)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {tokens.access_token}");
        }
        public static async void GetTokens()
        {
            var request = new RestRequest(Environment.GetEnvironmentVariable("OSU_OAUTH_URL"), Method.Post);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"));
            request.AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"));
            request.AddParameter("scope","public");
            var response = await new RestClient().ExecutePostAsync(request);
            Console.WriteLine("updated osu tokens");
            tokens = JsonConvert.DeserializeObject<OAuthTokens>(response.Content);

            File.WriteAllText(Global.config.osu_tokens_storage_path, response.Content);
        }

        public static async Task<ScoreExtended[]> GetUserRecentScores(string id, string mode)
        {
            var request = new RestRequest($"users/{id}/scores/recent", Method.Get);
            AddDefaultHeader(request);
            request.AddQueryParameter("limit","11"); //# Of scores you get from the Api
            request.AddQueryParameter("mode",mode);
            var response = await client.ExecuteGetAsync(request);

            Console.WriteLine("GetUserRecentScores : id : " + id + " mode : "  + mode);
            Console.WriteLine("GetUserRecentScores : response status code "+ response.StatusCode);
            
            if(response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ScoreExtended[]>(response.Content,  new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else 
            {
                Console.WriteLine("osu api response error content" + response.Content);
                Console.WriteLine("Couldn't get scores!");
                return [];
            }
        }

        public static uint GetXP(ScoreExtended score)
        {
            return (uint) Math.Ceiling(score.accuracy*score.beatmap.hit_length*score.beatmap.difficulty_rating);
        }

        public static async Task<Beatmap> GetBeatmapById(string beatmapId)
        {
            var request = new RestRequest($"/beatmaps/{beatmapId}", Method.Get);
            AddDefaultHeader(request);
            //request.AddQueryParameter("id",beatmapId);

            
            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("GetBeatmapById : response content "+ response.Content);
            Console.WriteLine("GetBeatmapById : osu api response status code "+ response.StatusCode);


            if(response.IsSuccessStatusCode)
            {
                var content = response.Content;
                content = content.Replace("cover@2x", "cover2x");
                content = content.Replace("card@2x", "cover2x");
                content = content.Replace("list@2x", "cover2x");
                content = content.Replace("slimcover@2x", "cover2x");
                var beatmap =  JsonConvert.DeserializeObject<Beatmap>(content,  new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                
                beatmap.beatmapset.covers.cover2x ??= beatmap.beatmapset.covers.cover2x.Replace("cover2x", "cover@2x");
                if(beatmap.beatmapset.covers.card2x != null) beatmap.beatmapset.covers.card2x = beatmap.beatmapset.covers.card2x.Replace("card2x", "card@2x");
                if(beatmap.beatmapset.covers.list2x != null) beatmap.beatmapset.covers.list2x = beatmap.beatmapset.covers.list2x.Replace("list2x", "list@2x");
                if(beatmap.beatmapset.covers.slimcover2x != null) beatmap.beatmapset.covers.slimcover2x = beatmap.beatmapset.covers.slimcover2x.Replace("slimcover", "slimcover@2x");
                return beatmap;
            }
            else 
            {
                Console.WriteLine("osu api response error content" + response.Content);
                return null;
            }
        }

        /*public static async Task<LookUpBeatmap> LookUpBeatmapById(string id)
        {

            var request = new RestRequest($"beatmaps/lookup", Method.Get);
            AddDefaultHeader(request);
            request.AddQueryParameter("id",id);


            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("GetBeatmapSetById : response content "+ response.Content);
            Console.WriteLine("GetBeatmapSetById : osu api response status code "+ response.StatusCode);

            if(response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<LookUpBeatmap>(response.Content,  new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else 
            {
                Console.WriteLine("osu api response error content" + response.Content);
                return null;
            }
        }*/

        /*public static async Task<Beatmap> GetBeatmapSetById(string beatmapsetId){
            Console.WriteLine("GetBeatmapSetById : id "+ beatmapsetId);

            var request = new RestRequest($"/beatmapsets/{beatmapsetId}", Method.Get);
            AddDefaultHeader(request);
            //request.AddQueryParameter("id",Convert.ToInt64(beatmapsetId));


            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("GetBeatmapSetById : response content "+ response.Content);
            Console.WriteLine("GetBeatmapSetById : osu api response status code "+ response.StatusCode);

            if(response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Beatmap>(response.Content,  new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else 
            {
                Console.WriteLine("osu api response error content" + response.Content);
                return null;
            }
        }*/
        public static async Task<bool> SendMessageToUser(string osuUserId, string message)
        {
            var request = new RestRequest($"chat/new", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {chat_tokens.access_token}");

            request.AddJsonBody(JsonConvert.SerializeObject(new {
                target_id=Convert.ToInt64(osuUserId),
                is_action=false,
                message,
            }));
            
            
            var response = await client.ExecutePostAsync(request);
            Console.WriteLine("osu api response status code " + response.StatusCode);
            
            if(!response.IsSuccessStatusCode)
                Console.WriteLine("osu api response error content" + response.Content);
            
            return response.IsSuccessStatusCode;
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

            chat_tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthTokens>(response.Content);
            File.WriteAllText(Global.config.osu_chat_tokens_storage_path, response.Content);


        }
    }
}
