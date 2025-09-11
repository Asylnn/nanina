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
        protected void Disconnect(ClientWebSocketResponse rawData)
        {
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if (session == null)
            { Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return; }
            session.UpdateUserId(null);
        }

        /*
            This is used to provide the session to the client. It also checks if the session has a user
            associated with it. In that case, it also send the user and some other data
        */
        protected void ProvideSessionToClient(ClientWebSocketResponse rawData)
        {
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if (session == null)
                session = Session.NewSession(ID);
            else
            {
                session.webSocketId = ID;
                DBUtils.Update(session);
                if (session.userId is not null)
                    ProvideUserToClient(session.userId);
            }
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideSession,
                data = JsonConvert.SerializeObject(session),
            }));
        }

        protected void ProvideUserToClient(string userId)
        {
            var user = DBUtils.Get<User>(x => x.Id == userId)!;
            if (user is null) return; //Shouldn't be necessary outside of a testing environnements
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
                data = JsonConvert.SerializeObject(user),
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideBanners,
                data = Global.bannersString,
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideDungeons,
                data = Global.dungeonsString,
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideResearchNodes,
                data = Global.researchNodesString,
            }));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideCraftingRecipes,
                data = Global.craftingRecipesString,
            }));
            if (user.isInDungeon)
            {
                var activeDungeon = DungeonManager.activeDungeons.Values.ToList().Find(dungeon => user.dungeonInstanceId == dungeon.instanceId)!;
                activeDungeon.sessionId = user.activeSessionId!;
                DungeonManager.UpdateDungeonOfClient(activeDungeon);
            }
            if (user.admin)
            {
                /*Send the full waifu, item, set and equipment database*/
                string[] dbNames = ["waifu", "item", "set", "equipment"];
                foreach (string dbName in dbNames)
                {
                    var data = File.ReadAllText($"../save/{dbName}.json");
                    Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
                    {
                        type = dbName switch
                        {
                            "waifu" => ServerResponseType.ProvideWaifuDB,
                            "item" => ServerResponseType.ProvideItemDB,
                            "set" => ServerResponseType.ProvideSetDB,
                            "equipment" => ServerResponseType.ProvideEquipmentDB,
                            _ => throw new NotImplementedException(),
                        },
                        data = data,
                    }));
                }
            }
        }
        protected void SendLoot(Loot[] loot, bool giveLoot = false)
        {
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = giveLoot ? ServerResponseType.ProvideAndGiveLoot : ServerResponseType.ProvideLoot,
                data = JsonConvert.SerializeObject(loot)
            }));
        }
    }
}
