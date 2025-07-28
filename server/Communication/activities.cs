using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.Dungeon;
using Nanina.UserData;
using System.Data.Common;
using System.Timers;
using Nanina.UserData.WaifuData;

namespace Nanina.Communication
{
    /*
        This file is for extending the partial class WS. Most files in Communication are for this purpose
        This file in particular is for managing the activities.
    */
    partial class WS : WebSocketBehavior
    {
        partial class ClientActivityRequest
        {
            public string waifuID;
            public ActivityType activityType;
        }
        protected (User user, Waifu waifu, bool validResult) CheckForActivityValidity(ClientWebSocketResponse rawData)
        {
            var activityRequest = JsonConvert.DeserializeObject<ClientActivityRequest>(rawData.data);

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return (null, null, false);}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return (null, null, false);}
            
            if(user.activities.Count >= user.maxConcurrentActivities)
                {Send(ClientNotification.NotificationData("Activities", "You reached the limit of waifus able to do an activity", 1)); return (null, null, false);}
            var waifuIndex = user.waifus.FindIndex(waifu => waifu.id == activityRequest.waifuID);
            if(waifuIndex == -1)
                {Send(ClientNotification.NotificationData("Activities", "You don't have this waifu", 1)); return (null, null, false);}
            var waifu = user.waifus[waifuIndex];
            if(waifu.isDoingSomething)
                {Send(ClientNotification.NotificationData("Activities", "Waifu is already doing something", 1)); return (null, null, false);}
            if(!Enum.IsDefined(activityRequest.activityType))
                {Send(ClientNotification.NotificationData("Activities", "activity doesn't exist", 1)); return (null, null, false);}

            return (user, waifu, true);
        }
        protected void SendWaifuToActivity(ClientWebSocketResponse rawData)
        {
            Console.WriteLine("SendWaifuToActivity");
            var (user, waifu, validResult) = CheckForActivityValidity(rawData);
            if(!validResult) return;
            Console.WriteLine("TEST PASSED");

            var activityRequest = JsonConvert.DeserializeObject<ClientActivityRequest>(rawData.data);

            waifu.isDoingSomething = true;
            Activity activity = new()
            {
                type = activityRequest.activityType,
                timeout = Global.baseValues.base_activity_length_in_milliseconds,
                waifuID = activityRequest.waifuID,
            };
            user.activities.Add(activity);


            var timer = new Activities.ActivityTimer(Global.baseValues.base_activity_length_in_milliseconds)
            {
                userId = user.Id,
                activityId = activity.id,
            };
            timer.Start();

            DBUtils.Update(user);Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
            Console.WriteLine("activity started");
        }

        protected void ClaimActivity(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}

            var activityIndex = user.activities.FindIndex(activity => activity.id == Convert.ToUInt64(rawData.data));
            if(activityIndex == -1)
                {Send(ClientNotification.NotificationData("Dungeon", "There is no activity with that id", 1)); return ;}
            var activity = user.activities[activityIndex];
            
            if(!activity.finished)
                {Send(ClientNotification.NotificationData("Dungeon", "This activity is not yet finished", 1)); return ;}

            var waifu = user.waifus.Find(waifu => waifu.id == activity.waifuID);
            waifu.isDoingSomething = false;
            Loot.GrantLoot(activity.loot, user);
            
            SendLoot([.. activity.loot]);

            user.activities.Remove(activity);
            DBUtils.Update(user);

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }
    }
}
