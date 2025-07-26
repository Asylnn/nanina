using WebSocketSharp.Server;
using Newtonsoft.Json;
using LiteDB;
using Nanina.Database;
using Nanina.Gacha;
using Nanina.Dungeon;
using Nanina.UserData;
using System.Data.Common;
using System.Timers;

namespace Nanina.Communication
{
    /*
        This file is for extending the partial class WS. Most files in Communication are for this purpose
        This file in particular is for managing the activities.
    */
    partial class WS : WebSocketBehavior
    {
        protected void SendWaifuToCafe(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return ;}
            
            if(user.activities.Count >= user.maxConcurrentActivities)
                {Send(ClientNotification.NotificationData("Activities", "You reached the limit of waifus able to do an activity", 1)); return ;}
            var waifuIndex = user.waifus.FindIndex(waifu => waifu.id == rawData.data);
            if(waifuIndex == -1)
                {Send(ClientNotification.NotificationData("Activities", "You don't have this waifu", 1)); return ;}
            var waifu = user.waifus[waifuIndex];
            if(waifu.isDoingSomething)
                {Send(ClientNotification.NotificationData("Activities", "Waifu is already doing something", 1)); return ;}

            waifu.isDoingSomething = true;
            Activity activity = new()
            {
                type = ActivityType.Cafe,
                timeout = Global.baseValues.cafe_length_in_milliseconds,
                waifuID = rawData.data,
            };
            user.activities.Add(activity);


            var tim = new Activities.ActivityTimer(Global.baseValues.cafe_length_in_milliseconds)
            {
                userId = user.Id,
                activityId = activity.id,
            };
            tim.Start();
            Global.activityTimers.Add(activity.id, tim);

            DBUtils.Update(user);Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
            /*Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "loot",
                data = JsonConvert.SerializeObject(loot)
            }));*/
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
