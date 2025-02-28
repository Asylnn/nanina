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
        public static RestRequest AddDefaultHeader(RestRequest request)
        {
            return request.AddHeader("Content-Type", "application/json")
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"Bearer {tokens.access_token}");
        }

        public static async void RefreshChatTokens()
        {
            var request = new RestRequest(Global.config.chat_osu_oauth_url, Method.Post);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json")
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("grant_type", "refresh_token")
                .AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"))
                .AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"))
                .AddParameter("refresh_token", chat_tokens.refresh_token);
                //.AddParameter("scope","public");
            var response = await new RestClient().ExecutePostAsync(request);

            Console.WriteLine("updated osu chat tokens");
            Console.WriteLine("osu api chat refresh response status code " + response.StatusCode);

            chat_tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthTokens>(response.Content);
            File.WriteAllText(Global.config.osu_chat_tokens_storage_path, response.Content);
        }
        public static async void GetTokens()
        {
            var request = new RestRequest(Global.config.osu_oauth_url, Method.Post);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json")
                .AddParameter("grant_type", "client_credentials")
                .AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"))
                .AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"))
                .AddParameter("scope","public");
            var response = await new RestClient().ExecutePostAsync(request);
            
            tokens = JsonConvert.DeserializeObject<OAuthTokens>(response.Content);

            File.WriteAllText(Global.config.osu_tokens_storage_path, response.Content);
            Console.WriteLine("updated osu tokens");
        }

        public static async Task<ScoreExtended[]> GetUserRecentScores(string id, string mode)
        {
            var request = new RestRequest($"users/{id}/scores/recent", Method.Get);
            AddDefaultHeader(request)
                .AddQueryParameter("limit", "1") //# Of scores you get from the Api
                .AddQueryParameter("mode", mode);
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
                Console.WriteLine("osu api GetUserRecentScores response error content" + response.Content);
                Console.WriteLine("Couldn't get scores!");
                return [];
            }
        }

        public static uint GetXP(ScoreExtended score)
        {
            /*star rating multiplicator   : (2 + e**(0.45*x))/3.57
            acc multiplicator           : (x^2) // (x/0.95)^1.2*(1/(1 + e^(-20*(x-0.65))))
            drain time multplicator     : ((x^0.4)/1.18)
            combo multiplicator         : (x^0.15)
            */
            /*
                To add : OD, CS buff, low AR buff
                HD : +10%?
            */
            Console.WriteLine(JsonConvert.SerializeObject(score.beatmapset.title));
            var star_rating_multiplicator = (2+Math.Pow(Math.E, 0.5*score.beatmap.difficulty_rating))/3.65;
            var acc_multiplicator = score.accuracy*score.accuracy;
            var drain_time_multiplicator = Math.Pow(score.beatmap.hit_length/60f, 0.4)/1.18f;
            var combo_multiplicator = Math.Pow(score.max_combo/(score.beatmap.count_circles + score.beatmap.count_sliders*2f + score.beatmap.count_spinners), 0.15);
            Console.WriteLine("star_rating_multiplicator : " + star_rating_multiplicator);
            Console.WriteLine("acc_multiplicator : " + acc_multiplicator);
            Console.WriteLine("drain_time_multiplicator : " + drain_time_multiplicator);
            Console.WriteLine("drain_time : " + score.beatmap.hit_length);
            Console.WriteLine("combo_multiplicator : " + combo_multiplicator);
            
            var xp =  (uint) Math.Ceiling(score.beatmap.hit_length*acc_multiplicator*drain_time_multiplicator*star_rating_multiplicator*combo_multiplicator*10);
            Console.WriteLine("xp : " + xp);
            Console.WriteLine("xp per minute : " + xp/(score.beatmap.hit_length/60f));
            return xp;
        }

        public static async Task<Beatmap> GetBeatmapById(string beatmapId)
        {
            var request = new RestRequest($"/beatmaps/{beatmapId}", Method.Get);
            AddDefaultHeader(request);

            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("GetBeatmapById : osu api response status code "+ response.StatusCode);

            
            if(!response.IsSuccessStatusCode)
                {Console.WriteLine("osu api GetBeatmapById response error content" + response.Content); return null;}
            
            var content = response.Content
                .Replace("cover@2x", "cover2x")
                .Replace("card@2x", "cover2x")
                .Replace("list@2x", "cover2x")
                .Replace("slimcover@2x", "cover2x");
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
            var request = new RestRequest($"chat/new", Method.Post)

                .AddHeader("Content-Type", "application/json")
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"Bearer {chat_tokens.access_token}")

                .AddJsonBody(JsonConvert.SerializeObject(new {
                    target_id=Convert.ToInt64(osuUserId),
                    is_action=false,
                    message,
                }));
            
            
            var response = await client.ExecutePostAsync(request);
            Console.WriteLine("osu api sendMessageToUser response status code " + response.StatusCode);

            if(!response.IsSuccessStatusCode)
                Console.WriteLine("osu api sendMessageToUser response error content" + response.Content);
            
            return response.IsSuccessStatusCode;
        }

        public static async void AuthorizeSelf(string code)
        {

            // for sending messages used for verifications, to get the code, check the following link :
            //https://osu.ppy.sh/oauth/authorize?client_id=422727&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5173&scope=chat.write
            //replace client_id with your osu client id, same with the redicted_uri
            
            var request = new RestRequest(Global.config.chat_osu_oauth_url, Method.Post);
            var redirect_uri = Global.config.dev ? Environment.GetEnvironmentVariable("DEV_OSU_REDIRECT_URI") : Environment.GetEnvironmentVariable("PROD_OSU_REDIRECT_URI") ;

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddHeader("Accept", "application/json")
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("client_id", Environment.GetEnvironmentVariable("OSU_CLIENT_ID"))
                .AddParameter("client_secret", Environment.GetEnvironmentVariable("OSU_CLIENT_SECRET"))
                .AddParameter("redirect_uri", redirect_uri)
                .AddParameter("code", code);

            


            var response = await new RestClient().ExecutePostAsync(request);

            Console.WriteLine("updated osu chat tokens");
            Console.WriteLine("osu api AuthorizeSelf response status code " + response.StatusCode);

            chat_tokens =  Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthTokens>(response.Content);
            File.WriteAllText(Global.config.osu_chat_tokens_storage_path, response.Content);


        }
    }
}
