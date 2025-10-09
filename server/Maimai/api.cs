using Nanina;
using Newtonsoft.Json;
using RestSharp;

namespace Maimai
{
    public static class Api
    {
        public static RestClient client = new RestClient(Global.config.maitea_api_url);
        public static RestRequest AddDefaultHeader(RestRequest request, string key)
        {
            return request.AddHeader("Content-Type", "application/json")
                .AddHeader("Accept", "application/json")
                .AddHeader("Authorization", $"Bearer {key}");
        }
        public static async void GetTracks()
        {
            Console.WriteLine("getting tracks...");

            var request = new RestRequest($"api/v1/tracks", Method.Get);
            AddDefaultHeader(request, Environment.GetEnvironmentVariable("MAITEA_AUTH_KEY")!);
            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("response status code "+ response.StatusCode);
            File.WriteAllText("../gettracks.json", response.Content);
        }

        public static async Task<bool> VerifyToken(string key)
        {
            var request = new RestRequest($"api/v1/tracks", Method.Get);
            AddDefaultHeader(request, key);
            var response = await client.ExecuteGetAsync(request);
            return response.IsSuccessStatusCode;
        }

        public static async Task<Score[]> GetRecentScores(string key, uint songID = 0, byte level = 0)
        {
            Console.WriteLine("getting tracks...");

            var request = new RestRequest($"api/v1/plays", Method.Get);
            AddDefaultHeader(request, key);
            if(songID != 0)
                request.AddQueryParameter("song", songID);
            if(level != 0)
                request.AddQueryParameter("level", level);
            var response = await client.ExecuteGetAsync(request);
            Console.WriteLine("response maimai GetRecentScores status code "+ response.StatusCode);
            Console.WriteLine("response maimai GetRecentScores content " + response.Content);
            if(response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ScoreResponse>(response.Content!)!.data;
            else
                return [];
            
            //File.WriteAllText("../getscores.json", response.Content);
        }

        public static uint GetXP(Score score)
        {
            Console.WriteLine("hello");
            Utils.ConsoleObject(score);
            var chart = Global.charts.Where(chart => chart.difficulty.ToLower() == score.difficulty_level.label.ToLower() && chart.songID == score.song.id).First();
            return (uint) Math.Floor(score.achievement/chart.data.maxPercent*chart.levelNum)*10;
        }
    }
}