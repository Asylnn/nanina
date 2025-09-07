using WebSocketSharp.Server;
using Newtonsoft.Json;
using Nanina.Database;
using Nanina.UserData.ItemData;
using Nanina.UserData.WaifuData;
using Nanina.UserData;
using Newtonsoft.Json.Bson;

namespace Nanina.Communication
{
    partial class WS : WebSocketBehavior
    {
        #pragma warning disable 0649
        protected class EquipItemFormat
        {
            public int equipmentId;
            public string? waifuId;
        }

        protected class UnequipItemFormat
        {
            public EquipmentPiece equipmentPiece;
            public string? waifuId;
        }
        #pragma warning restore 0649
        protected async void UpdateOsuId(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
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
            DBUtils.Update(user);
            
        }
        protected void VerifyOsuId(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(Utils.GetTimestamp() - user.verification.osuVerificationCodeTimestamp > Global.baseValues.time_limit_for_osu_code_verification_in_milliseconds)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code expired", 1)); return;}
            if(user.verification.osuVerificationCode != rawData.data)
                {Send(ClientNotification.NotificationData("Update osu ID", "The code is wrong", 1)); return;}

            user.verification.osuVerificationCode = null;
            user.verification.osuVerificationCodeTimestamp = 0;
            user.verification.isOsuIdVerified = true;

            DBUtils.Update(user);
            Send(ClientNotification.NotificationData("Update osu ID", "You successfully modified your osu id!", 1));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.OsuIdUpdateSuccess,
                data = ""
            }));
        }
        protected void UpdateTheme(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user != null){
                user.theme = rawData.data;
                DBUtils.Update(user);
            }
        }
        protected void EquipItem(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(Utils.TryDeserialize<EquipItemFormat>(rawData.data, out var clientData) == false)
                {Send(ClientNotification.NotificationData("User", "Invalid data (client Data is null)", 1)); return ;}
            if(clientData.waifuId == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data, waifuId is null", 1)); return ;}
            
           if(user.waifus.TryGet(clientData.waifuId, out var waifu) == false)
                {Send(ClientNotification.NotificationData("Equip", "The waifu you tried to equip the item with doesn't exist", 1)); return;}

            if(user.inventory.equipment.TryGet(clientData.equipmentId, out var equipment) == false)
                {Send(ClientNotification.NotificationData("Equip", "The item you tried to equip doesn't exist", 1)); return;}

            var oldEquipment = waifu.Equip(equipment);
            user.inventory.equipment.Remove(equipment.inventoryId);
            if(oldEquipment is not null)
                user.inventory.AddEquipment(oldEquipment);

            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ConfirmEquip,
                data = clientData.equipmentId.ToString(),
            }));
        }

        protected void UnequipItem(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user is null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            if(Utils.TryDeserialize<UnequipItemFormat>(rawData.data, out var clientData) == false)
                {Send(ClientNotification.NotificationData("User", "Invalid data (client Data is null)", 1)); return ;}
            if(clientData.waifuId is null)
                {Send(ClientNotification.NotificationData("User", "Invalid data, waifu Id is null", 1)); return ;}
            if(user.waifus.TryGet(clientData.waifuId, out var waifu) == false)
                {Send(ClientNotification.NotificationData("Equip", "The waifu you tried to unequip the item with doesn't exist", 1)); return;}
            if(Enum.IsDefined(clientData.equipmentPiece) == false)
                {Send(ClientNotification.NotificationData("Equip", "The equipment piece enum doesn't exist", 1)); return;}

            var oldEquipment = waifu.Unequip(clientData.equipmentPiece);

            if(oldEquipment is not null)
                user.inventory.AddEquipment(oldEquipment);
            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ConfirmUnequip,
                data = ((int)clientData.equipmentPiece).ToString(),
            }));
        }

        protected async void VerifyMaimaiToken(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}

            var success = await Maimai.Api.VerifyToken(rawData.data);
            

            if(!success)
                {Send(ClientNotification.NotificationData("Update osu ID", "We can't find the user associated with that id! (or other server side issues)", 1)); return;}
                
            
            user.tokens.maimai_token = rawData.data;
            user.verification.isMaimaiTokenVerified = true;
            DBUtils.Update(user);

            Send(ClientNotification.NotificationData("Update Maimai token", "You successfully modified your maimai token!", 1));
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.MaimaiTokenUpdateSuccess,
                data = ""
            }));
        }

        protected void GetReward(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}

            var lvl = Convert.ToByte(rawData.data);

            if(lvl > user.lvl)
                {Send(ClientNotification.NotificationData("User", "You haven't unlocked this level!", 1)); return ;}

            if(!user.CheckRewardAvailability(lvl))
                {Send(ClientNotification.NotificationData("User", "You already collected the rewards for this level", 1)); return ;}
                
            user.lvlRewards += Convert.ToInt64(Math.Pow(2, lvl));

            Loot.GrantLoot(Global.userLevelRewards[lvl - 2], user);

            SendLoot([.. Global.userLevelRewards[lvl-2]]);
            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
                data = JsonConvert.SerializeObject(user) 
            }));
        }

        protected void UpdatePreferedGame(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            user.preferedGame = (Game) Convert.ToInt16(rawData.data);
            DBUtils.Update(user);
        }

        protected void UseUserConsumable(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}

            if(user.inventory.items.TryGet(Convert.ToInt16(rawData.data), out var item) == false)
                {Send(ClientNotification.NotificationData("Equip", "You don't have the item you tried to use", 1)); return;}

            user.UseUserConsumable(item);
            user.inventory.RemoveItem(item);

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
                data = JsonConvert.SerializeObject(user) 
            }));

            DBUtils.Update(user);
        }

        protected void UpdateUserWaifus(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
            {
                Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); 
                return;
            }
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            user.waifus = JsonConvert.DeserializeObject<Dictionary<string, Waifu>>(rawData.data)!;
            Send(ClientNotification.NotificationData("admin", "modified waifus", 1));
            DBUtils.Update(user);
        }

        protected void UpdateUserInventory(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
            {
                Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); 
                return;
            }
            if(!user.admin)
                {Send(ClientNotification.NotificationData("admin", "You don't have the permissions for this action!", 0)); return;}

            var inventory = JsonConvert.DeserializeObject<Inventory>(rawData.data)!;
            user.inventory = inventory;
            Send(ClientNotification.NotificationData("admin", "modified inventory", 1));
            DBUtils.Update(user);
        }

        protected void UpgradeEquipment(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if (user is null)
                { Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return; }
            if(user.inventory.equipment.TryGet(Convert.ToInt32(rawData.data), out var equipment) == false)
                { Send(ClientNotification.NotificationData("User", "This equipment does not exist!", 1)); return; }
                
            equipment.LevelUp();
            Send(ClientNotification.NotificationData("admin", "temp mes", 1));
            DBUtils.Update(user);
        }

        protected void BecomeAdmin(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(Global.config.dev == false) 
            {
                Send(ClientNotification.NotificationData("Admin", "The server is in production, you can't use this!", 1)); 
                return;
            }
            if(user == null) 
            {
                Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); 
                return;
            }
            user.admin = true;
            Send(ClientNotification.NotificationData("admin", "temp mes", 1));
            DBUtils.Update(user);
        }

        protected void UpdateLocale(ClientWebSocketResponse rawData)
        {
            var session = DBUtils.Get<Session>(session => session.id == rawData.sessionId);
            if(session is null)
                {Send(ClientNotification.NotificationData("Update Locale", "You can't perform this action without a valid session", 1)); return ;}
            if(Global.config.available_languages.Contains(rawData.data) == false)
                {Send(ClientNotification.NotificationData("Update Locale", "This language is not available", 1)); return ;}
            session.locale = rawData.data;
            if(session.userId is not null){
                var user = DBUtils.Get<UserData.User>(x => x.Id == session.userId)!;
                user.locale = rawData.data;
                DBUtils.Update(user);
            }

            DBUtils.Update(session);
        }
    }
}
