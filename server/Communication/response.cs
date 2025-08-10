using WebSocketSharp.Server;
using WebSocketSharp;
using Nanina.Database;
using System.Data.Common;
using LiteDB;
using System.Diagnostics;

namespace Nanina.Communication
{
    
    partial class WS : WebSocketBehavior
    {
        //static private List<WS> webSocketConnections = [];
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("DATA : " + (string) e.Data); 
            ClientWebSocketResponse? rawData = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientWebSocketResponse>(e.Data);
            if(rawData is null) return;

            if(Global.config.dev)
                ProcessMessage(rawData);
            else 
            {
                try
                {
                    ProcessMessage(rawData);
                }
                catch(Exception err)
                {
                    Console.Error.WriteLine(err.Message);
                    using var stream = File.AppendText("../errors.log");
                    stream.WriteLine(err.Message);
                }
            }
            
            
        }

        protected void ProcessMessage(ClientWebSocketResponse rawData)
        {
            
            switch (rawData.type) {

                //Recieve a request from the client to get a user from a sessionId stored in cookies.
                //It check the database for that sessionId and return, if found, the user associated with that session ID.
                case ClientResponseType.UpdateTheme: 
                    UpdateTheme(rawData);
                    break;
                
                case ClientResponseType.UpdateOsuId:  
                    UpdateOsuId(rawData);
                    break;
                case ClientResponseType.UpdateLocale:
                    UpdateLocale(rawData);
                    break;
                case ClientResponseType.UpdatePreferedGame:
                    UpdatePreferedGame(rawData);
                    break;
                case ClientResponseType.UpdateWaifuDB: 
                    UpdateWaifuDatabase(rawData);
                    break;
                case ClientResponseType.UpdateItemDB: 
                    UpdateItemDatabase(rawData);
                    UpdateEquipmentDatabase(rawData);
                    break;
                case ClientResponseType.UpdateSetDB:
                    UpdateSetDatabase(rawData);
                    break;
                case ClientResponseType.StartFight: 
                    StartFight(rawData);
                    break;
                case ClientResponseType.ClaimFight:
                    ClaimFight(rawData);
                    break;
                case ClientResponseType.GetSession:
                    ProvideSessionToClient(rawData);
                    break;
                case ClientResponseType.AddBeatmap: 
                    AddMapToDatabase(rawData);
                    break;
                case ClientResponseType.ConnectWithDiscord:
                    DiscordLogin(rawData);
                    break;
                case ClientResponseType.GetPullResults:
                    Pull(rawData);
                    break;
                case ClientResponseType.VerifyOsuId:
                    VerifyOsuId(rawData);
                    break;
                case ClientResponseType.GetMapData:
                    SendMapToClient(rawData);
                    break;
                case ClientResponseType.StartDungeon:
                    StartDungeon(rawData);
                    break;
                case ClientResponseType.StopDungeon:
                    StopDungeon(rawData);
                    break;
                case ClientResponseType.UnequipItem:
                    UnequipItem(rawData);
                    break;
                case ClientResponseType.Logout:
                    Disconnect(rawData);
                    break;
                case ClientResponseType.EquipItem:
                    EquipItem(rawData);
                    break;
                case ClientResponseType.VerifyMaimaiToken:
                    VerifyMaimaiToken(rawData);
                    break;
                case ClientResponseType.GetLevelRewardsData:
                    GetLevelRewards(rawData);
                    break;
                case ClientResponseType.GetLevelRewards:
                    GetReward(rawData);
                    break;
                case ClientResponseType.UseUserConsumable:
                    UseUserConsumable(rawData);
                    break;
                case ClientResponseType.UpdateUserWaifu:
                    UpdateUserWaifus(rawData);
                    break;
                case ClientResponseType.ClaimDungeonFight:
                    ClaimDungeonFight(rawData);
                    break;
                case ClientResponseType.UpdateUserInventory:
                    UpdateUserInventory(rawData);
                    break;
                case ClientResponseType.UpgradeEquipment:
                    UpgradeEquipment(rawData);
                    break;
                case ClientResponseType.BecomeAdmin:
                    BecomeAdmin(rawData);
                    break;
                case ClientResponseType.SendWaifuToActivity:
                    SendWaifuToActivity(rawData);
                    break;
                case  ClientResponseType.ClaimActivity:
                    ClaimActivity(rawData);
                    break;
                case ClientResponseType.CheckContinuousFight:
                    CheckContinuousFight(rawData);
                    break;
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            /*Get session col
            to find one session and empty its websocket id*/
            /*var session = DBUtils.Get<Session>(session => session.webSocketId == ID);
            if(session is not null)
            {
                session.UpdateWebSocketId(null);
            }*/
            Console.WriteLine("Bye :'(");
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Hello <3");
        }
    }
}
