
namespace Nanina.Communication
{
    public class ClientWebSocketResponse
    {
        public string type;
        public string data;
        public string sessionId;
        public string? userId;
    }

    public class ServerWebSocketResponse
    {
        public string type;
        public string data;
    }
}
