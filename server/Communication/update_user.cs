using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected async void UpdateOsuId(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            var rng = new Random();
            var code = (rng.Next(899_999) + 100_000).ToString();
            var success = await Osu.Api.SendMessageToUser(rawData.data, code);
            Console.WriteLine("code "+ code);
            

            if(!success && !user.admin){
                Send(ClientNotification.NotificationData("Update osu ID", "We can't find the user associated with that id! (or other server side issues)", 1)); return;
            }
            else {
                user.ids.osuId = rawData.data;
                user.verification.osuVerificationCode = code;
                user.verification.osuVerificationCodetimestamp = Utils.GetTimestamp();
                user.verification.isOsuIdVerified = false;
                DBUtils.UpdateUser(user);
            }
        }
        protected void VerifyOsuId(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(Utils.GetTimestamp() - user.verification.osuVerificationCodetimestamp > Global.baseValues.time_limit_for_osu_code_verification_in_milliseconds)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code expired", 1)); return;}
            
            if(user.verification.osuVerificationCode != rawData.data)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code is wrong", 1)); return;}

            user.verification.osuVerificationCode = null;
            user.verification.osuVerificationCodetimestamp = 0;
            user.verification.isOsuIdVerified = true;

            DBUtils.UpdateUser(user);
            Send(ClientNotification.NotificationData("Update osu ID", "You successfully modified your osu id!", 1));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }
        protected void UpdateTheme(ClientWebSocketResponse rawData){
            var user = DBUtils.GetUser(rawData.userId);
            if(user != null){
                user.theme = rawData.data;
                DBUtils.UpdateUser(user);
            }   
        }
    }
}
