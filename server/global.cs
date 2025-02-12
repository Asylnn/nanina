using LiteDB;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace Nanina
{
    public static class Global {
        public static readonly Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../config.json"));
        public static readonly BaseValues baseValues = JsonConvert.DeserializeObject<BaseValues>(File.ReadAllText("../baseValues.json"));
        public static WebSocketServer ws;
    }
}
