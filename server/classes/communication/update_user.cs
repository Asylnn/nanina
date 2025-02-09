using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected async void UpdateOsuId(ClientWebSocketResponse rawData){
        Console.WriteLine("rawData : " + JsonConvert.SerializeObject(rawData));
        var user = DBUtils.GetUser(rawData.id);
        var rng = new Random();
        var code = rng.Next(999999).ToString();
        var success = await OsuApi.SendMessageToUser(rawData.data, code);
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
        var user = DBUtils.GetUser(rawData.id);
        Console.WriteLine(user.verification.osuVerificationCodetimestamp);
        Console.WriteLine(user.verification);
        Console.WriteLine(JsonConvert.SerializeObject(Global.config));
        Console.WriteLine(Global.config);
        if(Utils.GetTimestamp() - user.verification.osuVerificationCodetimestamp > Global.config.time_limit_for_osu_code_verification_in_milliseconds)
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
        var user = DBUtils.GetUser(rawData.id);
        user.theme = rawData.data;
        DBUtils.UpdateUser(user);
    }
}