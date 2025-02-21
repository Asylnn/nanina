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

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if (!user.admin)
                {Send(ClientNotification.NotificationData("Admin", "You don't have the permissions for this action!", 0)); return;}

            var map = await Osu.Api.GetBeatmapById(rawData.data);
            if(map == null) 
                {Send(ClientNotification.NotificationData("Admin", "This beatmap doesn't exist!", 1)); return;}


            /*Get maps col
            If maps col not empty, test if it's already inside
            if it's not inside, insert it in the db
            */
            if(DBUtils.isMapADuplicate(map)){Send(ClientNotification.NotificationData("Admin", "The beatmap is already in the database!", 1)); return ;} //If the map is already on the data base, don't add it again.
            DBUtils.Insert(map);
            Console.WriteLine("Adding map : " + map.id);
            Send(ClientNotification.NotificationData("Admin", "Added the beatmap into the database!", 3));
            
        }
    }
}







