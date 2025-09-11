using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.Osu;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    
    {
        #pragma warning disable 0649
        protected class AddMapRequest
        {
            public string? id;
            public NaninaStdTag tag;
        }
        #pragma warning restore 0649
        protected async void AddMapToDatabase(ClientWebSocketResponse rawData){

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user is null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this action while not being connected", 1)); return ;}
            if (user.admin == false)
                {Send(ClientNotification.NotificationData("Admin", "You don't have the permissions for this action!", 0)); return;}
            if(Utils.TryDeserialize<AddMapRequest>(rawData.data, out var request) == false)
                {Send(ClientNotification.NotificationData("Admin", "request is null", 1)); return;}
            var map = await Osu.Api.GetBeatmapById(request.id!);
            if(map == null) 
                {Send(ClientNotification.NotificationData("Admin", "either this beatmap doesn't exist, or the osu api keys are wrong/expired", 1)); return;}


            /*
                Get maps collection, test if the map is already inside. if it's not, insert it.
            */

            if(DBUtils.isMapADuplicate(map)){Send(ClientNotification.NotificationData("Admin", "The beatmap is already in the database!", 1)); return ;} //If the map is already on the data base, don't add it again.
            map.nanina_tag = request.tag;
            DBUtils.Insert(map);
            Console.WriteLine("Adding map : " + map.id);
            Send(ClientNotification.NotificationData("Admin", "Added the beatmap into the database!", 3));
            
        }
    }
}







