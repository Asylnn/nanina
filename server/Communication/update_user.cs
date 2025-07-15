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
                type = "user",
                data = JsonConvert.SerializeObject(user) 
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
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            var clientData = JsonConvert.DeserializeObject<EquipItemFormat>(rawData.data);
            if(clientData == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data", 1)); return ;}
            if(clientData.waifuId == null || clientData.equipmentId == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data", 1)); return ;}
            
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

            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
            }));
        }

        protected void UnequipItem(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); return ;}
            
            var clientData = JsonConvert.DeserializeObject<UnequipItemFormat>(rawData.data);
            if(clientData == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data", 1)); return ;}
            if(clientData.waifuId == null || clientData.equipmentPiece == null)
                {Send(ClientNotification.NotificationData("User", "Invalid data", 1)); return ;}
            

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

            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
                data = JsonConvert.SerializeObject(user) 
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
                type = "user",
                data = JsonConvert.SerializeObject(user) 
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

        
            foreach(var reward in Global.userLevelRewards[lvl-2])
            {
                switch(reward.lootType){
                    case LootType.GC:
                        user.gacha_currency += reward.amount;
                        break;
                    case LootType.Item:
                        user.inventory.AddItem(reward.item);
                        break;
                }
            }
            SendLoot([.. Global.userLevelRewards[lvl-2]]);
            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
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

            var itemIndex = user.inventory.userConsumable.FindIndex(uc => uc.inventoryId == Convert.ToUInt32(rawData.data));
            if(itemIndex == -1)
                {Send(ClientNotification.NotificationData("Equip", "You don't have the item you tried to use", 1)); return;}

            var item = user.inventory.userConsumable[itemIndex];

            user.UseItem(item);
            user.inventory.RemoveItem(item);

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = "user",
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

            user.waifus = JsonConvert.DeserializeObject<List<Waifu>>(rawData.data);
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

            var inventory = JsonConvert.DeserializeObject<Inventory>(rawData.data);
            user.inventory = inventory;
            Send(ClientNotification.NotificationData("admin", "modified inventory", 1));
            DBUtils.Update(user);
        }

        protected void UpgradeEquipment(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null) 
            {
                Send(ClientNotification.NotificationData("User", "You can't perform this account with being connected!", 1)); 
                return;
            }
            var equipmentIndex = user.inventory.equipment.FindIndex(equipment => equipment.inventoryId == Convert.ToUInt32(rawData.data));
            if(equipmentIndex == -1)
            {
                Send(ClientNotification.NotificationData("User", "This equipment does not exist!", 1)); 
                return;
            }
            Console.WriteLine(equipmentIndex);
            Console.WriteLine(user.inventory.equipment.Count);
            user.inventory.equipment[equipmentIndex].LevelUp();
            Send(ClientNotification.NotificationData("admin", "temp mes", 1));
            DBUtils.Update(user);
        }   
    }
}
