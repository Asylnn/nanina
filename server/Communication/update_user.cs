using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.UserData.ItemData;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        protected async void UpdateOsuId(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            var code = (new Random().Next(899_999) + 100_000).ToString();
            var success = await Osu.Api.SendMessageToUser(rawData.data, code);
            Console.WriteLine("code " + code);
            

            if(!success && !user.admin)
                {Send(ClientNotification.NotificationData("Update osu ID", 
                "We can't find the user associated with that id! (or other server side issues)", 1)); return;}
            
            user.ids.osuId = rawData.data;
            user.verification.osuVerificationCode = code;
            user.verification.osuVerificationCodeTimestamp = Utils.GetTimestamp();
            user.verification.isOsuIdVerified = false;
            DBUtils.UpdateUser(user);
            
        }
        protected void VerifyOsuId(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(Utils.GetTimestamp() - user.verification.osuVerificationCodeTimestamp > Global.baseValues.time_limit_for_osu_code_verification_in_milliseconds)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code expired", 1)); return;}
            if(user.verification.osuVerificationCode != rawData.data)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code is wrong", 1)); return;}

            user.verification.osuVerificationCode = null;
            user.verification.osuVerificationCodeTimestamp = 0;
            user.verification.isOsuIdVerified = true;

            DBUtils.UpdateUser(user);
            Send(ClientNotification.NotificationData("Update osu ID", "You successfully modified your osu id!", 1));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }
        protected void UpdateTheme(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user != null){
                user.theme = rawData.data;
                DBUtils.UpdateUser(user);
            }
        }

        protected class EquipItemFormat
        {
            public int equipmentId;
            public string waifuId;
        }

        protected class UnequipItemFormat
        {
            public EquipmentPiece equipmentPiece;
            public string waifuId;
        }
        protected void EquipItem(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            var clientData = JsonConvert.DeserializeObject<EquipItemFormat>(rawData.data);

            var waifu = user.waifus.Find(waifu => clientData.waifuId == waifu.id);
            if(waifu is null)
                {Send(ClientNotification.NotificationData("Equip", "The waifu you tried to equip the item with doesn't exist", 1)); return;}
            var itemIndex = user.inventory.equipment.FindIndex(item => clientData.equipmentId == item.inventoryId);
            if(itemIndex == -1)
                {Send(ClientNotification.NotificationData("Equip", "The item you tried to equip doesn't exist", 1)); return;}

            var oldEquipment = waifu.Equip(user.inventory.equipment[itemIndex]);
            user.inventory.equipment.RemoveAt(itemIndex);
            if(oldEquipment is not null)
                user.inventory.AddEquipment(oldEquipment);

            DBUtils.UpdateUser(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }

        protected void UnequipItem(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            var clientData = JsonConvert.DeserializeObject<UnequipItemFormat>(rawData.data);

            var waifu = user.waifus.Find(waifu => clientData.waifuId == waifu.id);
            if(waifu is null)
                {Send(ClientNotification.NotificationData("Equip", "The waifu you tried to equip the item with doesn't exist", 1)); return;}

            Equipment oldEquipment = null;
            switch(clientData.equipmentPiece)
            {
                case EquipmentPiece.Weapon:
                    oldEquipment = waifu.equipment.weapon;
                    waifu.equipment.weapon = null;
                    break;
                case EquipmentPiece.Dress:
                    oldEquipment = waifu.equipment.dress;
                    waifu.equipment.dress = null;
                    break;
                case EquipmentPiece.Accessory:
                    oldEquipment = waifu.equipment.accessory;
                    waifu.equipment.accessory = null;
                    break;
                default:
                    Send(ClientNotification.NotificationData("Equip", "Wrong equipment piece", 1));
                    break;
            }
            if(oldEquipment is not null)
                user.inventory.AddEquipment(oldEquipment);

            DBUtils.UpdateUser(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }

        protected async void VerifyMaimaiToken(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.GetUser(rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}

            var success = await Maimai.Api.VerifyToken(rawData.data);
            

            if(!success)
                {Send(ClientNotification.NotificationData("Update osu ID", "We can't find the user associated with that id! (or other server side issues)", 1)); return;}
                
            
            user.tokens.maimai_token = rawData.data;
            user.verification.isMaimaiTokenVerified = true;
            DBUtils.UpdateUser(user);

            Send(ClientNotification.NotificationData("Update Maimai token", "You successfully modified your maimai token!", 1));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }
    }
}
