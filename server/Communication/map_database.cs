using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Osu;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected async void AddMapToDatabase(ClientWebSocketResponse rawData){

            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if (!user.admin)
                {Send(ClientNotification.NotificationData("Admin", "You don't have the permissions for this action!", 0)); return;}

            var map = await Osu.Api.GetBeatmapById(rawData.data);
            if(map == null) 
                {Send(ClientNotification.NotificationData("Admin", "This beatmap doesn't exist!", 1)); return;}

            var mapsCol = DBUtils.GetCollection<Beatmap>();

            if(mapsCol.Count() != 0 ){
                if(mapsCol.Exists(x => x.id == map.id)){Send(ClientNotification.NotificationData("Admin", "The beatmap is already in the database!", 1)); return ;} //If the map is already on the data base, don't add it again.
            }
            mapsCol.Insert(map);
            mapsCol.EnsureIndex(x => x.id, true);
            mapsCol.EnsureIndex(x => x.difficulty_rating);
            Console.WriteLine("Adding map : " + map.id);
            Send(ClientNotification.NotificationData("Admin", "Added the beatmap into the database!", 3));
            
        }
    }
}







