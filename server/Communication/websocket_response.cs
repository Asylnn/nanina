
namespace Nanina.Communication
{
    /*
        The format which the Client uses to send information to the server
    */
    public class ClientWebSocketResponse
    {
        public string type;
        public string data;
        public string sessionId;
        public string userId;
    }

    /*
        The format which the Server uses to send back information to the client
    */
    public class ServerWebSocketResponse
    {
        public string type;
        public string data;
    }
}
