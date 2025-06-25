using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.Osu;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected class AddMapRequest
        {
            public string id;
            public NaninaStdTag tag;
        }
        protected async void AddMapToDatabase(ClientWebSocketResponse rawData){

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if (!user.admin)
                {Send(ClientNotification.NotificationData("Admin", "You don't have the permissions for this action!", 0)); return;}

            var request = JsonConvert.DeserializeObject<AddMapRequest>(rawData.data);

            var map = await Osu.Api.GetBeatmapById(request.id);
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







