
namespace Nanina.Communication
{
    /*
        The format which the Client uses to send information to the server
    */
    public class ClientWebSocketResponse
    {
        public required string type;
        public required string data;
        public required string sessionId;
        public required string userId;
    }

    /*
        The format which the Server uses to send back information to the client
    */
    public class ServerWebSocketResponse
    {
        public required string type;
        public required string data;
    }
}
