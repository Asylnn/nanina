using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using LiteDB;
using RestSharp;

partial class WS : WebSocketBehavior
{
    protected async void UpdateOsuId(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        var rng = new Random();
        var code = rng.Next(999999).ToString();
        var success = await OsuApi.SendMessageToUser(rawData.data, code);
        Console.WriteLine("code"+ code);
        user.verificationCodes.osuVerificationCode = code;
        user.verificationCodes.osuVerificationCodetimestamp = long.Parse(Utils.GetTimestamp());
        DBUtils.UpdateUser(user);

        if(!success){
            Send(ClientNotification.NotificationData("Update osu ID", "We can't find the user associated with that id! (or other server side issues)", 1)); return;
        }
    }
    protected void VerifyOsuId(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        Console.WriteLine(user.verificationCodes.osuVerificationCodetimestamp);
        Console.WriteLine(user.verificationCodes);
        Console.WriteLine(JsonConvert.SerializeObject(Global.config));
        Console.WriteLine(Global.config);
        if(long.Parse(Utils.GetTimestamp()) - user.verificationCodes.osuVerificationCodetimestamp > Global.config.time_limit_for_osu_code_verification_in_milliseconds)
            {Send(ClientNotification.NotificationData("Update osu ID", "The code expired", 1)); return;}
        
        if(user.verificationCodes.osuVerificationCode != rawData.data)
            {Send(ClientNotification.NotificationData("Update osu ID", "The code is wrong", 1)); return;}

        user.verificationCodes.osuVerificationCode = null;
        user.verificationCodes.osuVerificationCodetimestamp = 0;
        DBUtils.UpdateUser(user);
        Send(ClientNotification.NotificationData("Update osu ID", "You successfully modified your osu id!", 1));
    }
    protected void UpdateTheme(ClientWebSocketResponse rawData){
        var user = DBUtils.GetUser(rawData.id);
        user.theme = rawData.data;
        DBUtils.UpdateUser(user);
    }
}