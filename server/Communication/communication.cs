using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.Dungeon;
using Nanina.UserData;
using System.Data.Common;

namespace Nanina.Communication
{
    /*
        This file is for extending the partial class WS. Most files in Communication are for this purpose
        This file in particular is for managing the client session.
    */
    partial class WS : WebSocketBehavior
    {
        /*
            This function is called when the client click on the disconnect button.
            It remove the userId from the session.
        */
        protected void Disconnect(ClientWebSocketResponse rawData){
            var session = DBUtils.GetSession(rawData.sessionId);
            if(session == null) 
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}
            session.UpdateUserId(null);
        }

        /*
            This is used to provide the session to the client. It also checks if the session has a user
            associated with it. In that case, it also send the user and some other data
        */
        protected void ProvideSessionToClient(ClientWebSocketResponse rawData)
        {
            var session = DBUtils.GetSession(rawData.sessionId);
            if(session == null)
                session = Session.NewSession(ID);
            else
            {
                Console.WriteLine(ID);
                session.UpdateWebSocketId(ID);
                Console.WriteLine(JsonConvert.SerializeObject(session));
                if(session.userId != null)
                    ProvideUserToClient(session.userId);
            }
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                type = "session",
                data = JsonConvert.SerializeObject(session),
            }));
        }

        protected void ProvideUserToClient(string userId)
        {
            var user = DBUtils.GetUser(userId);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse {
                type = "user",
                data = JsonConvert.SerializeObject(user),
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "get banners",
                data = JsonConvert.SerializeObject(GachaManager.banners),
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "get dungeons",
                data = JsonConvert.SerializeObject(DungeonManager.dungeons),
            }));
            if(user.admin)
            {
                ProvideItemDatabase(userId);
                ProvideSetDatabase(userId);
                ProvideWaifuDatabase(userId);
            }
        }
        protected void SendLoot(Loot[] loot)
        {
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "loot",
                data = JsonConvert.SerializeObject(loot)
            }));
        }
    }
}
