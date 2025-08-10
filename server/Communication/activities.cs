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
using Nanina.Crafting;

namespace Nanina.Communication
{
    /*
        This file is for extending the partial class WS. Most files in Communication are for this purpose
        This file in particular is for managing the activities.
    */
    partial class WS : WebSocketBehavior
    {
        #pragma warning disable 0649
        public class ClientActivityRequest
        {
            public string? waifuID;
            public ActivityType activityType;
        }

        public class ClientResearchRequest
        {
            public string? waifuID;
            public string? researchID;
            public ActivityType activityType;
        }

        public class CraftingRequest
        {
            public ushort id;
            public ushort quantity;
        }

        public class ClientCraftingRequest
        {
            public string? waifuID;
            public ActivityType activityType;
            public List<CraftingRequest?>? craftingList;
        }

        #pragma warning restore 0649
        protected (User?, Waifu?, bool validResult) CheckForActivityValidity(ClientWebSocketResponse rawData)
        {
            var activityRequest = JsonConvert.DeserializeObject<ClientActivityRequest>(rawData.data);

            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return (null, null, false);}
            var session = DBUtils.Get<Session>(x => x.id == rawData.sessionId);
            if(session == null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this action without a valid session", 1)); return (null, null, false);}
            if(activityRequest is null)
                {Send(ClientNotification.NotificationData("Activities", "activityRequest is null!", 1)); return (null, null, false);}
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

        protected (bool, ResearchNode?) CheckForResearchValidity(User user, string data)
        {
            var researchRequest = JsonConvert.DeserializeObject<ClientResearchRequest>(data)!;
            
            var researchNode = Global.researchNodes.Find(RN => RN.id == researchRequest.researchID);
            if(researchNode == null)
                {Send(ClientNotification.NotificationData("Activities", "this research doesn't exist", 1)); return (false, null);}

            if(! researchNode.requirements.All(requirement => user.completedResearches.Any(completedResearch => completedResearch.Key == requirement)))
                {Send(ClientNotification.NotificationData("Activities", "you didn't already complete all the requirements", 1)); return (false, null);}

            if(! researchNode.infinite && user.completedResearches.Any(completedResearch => completedResearch.Key == researchNode.id))
                {Send(ClientNotification.NotificationData("Activities", "you already did the research (and it's not an infinite research)", 1)); return (false, null);}

            return (true, researchNode);

        }

        protected (bool, Craft?) CheckForCraftingValidity(User user, string data)
        {   
            
            var craftingRequest = JsonConvert.DeserializeObject<ClientCraftingRequest>(data)!;
            if(craftingRequest.craftingList is null)
                    {Send(ClientNotification.NotificationData("Activities", "the crafting list is null!", 1)); return (false, null);}
            if(craftingRequest.craftingList.Count == 0)
                {Send(ClientNotification.NotificationData("Activities", "one of the craft is null!", 1)); return (false, null);}
            //We merge all the crafting requests into a single  craft object
            Craft craftMerge = new ();
            foreach(var craft in craftingRequest.craftingList)
            {
                if(craft is null)
                    {Send(ClientNotification.NotificationData("Activities", "one of the craft is null!", 1)); return (false, null);}
                if(craft.quantity <= 0)
                    {Send(ClientNotification.NotificationData("Activities", "you craft something 0 or negatives times", 1)); return (false, null);}
                
                var fullCraft = Global.craftingRecipes.Find(cr => cr.id == craft.id);

                if(fullCraft == null)
                    {Send(ClientNotification.NotificationData("Activities", "the craft does not exist", 1)); return (false, null);}

                var fullCraftCopy = Utils.DeepCopyReflection(fullCraft)!;
                if(fullCraftCopy.ingredients.Count != 0)
                    fullCraftCopy.ingredients.First().Test();

                /*Each ingredient and result quantity is multiplied by the craft quantity*/
                fullCraftCopy.ingredients.ForEach(ingredient => ingredient.quantity *= craft.quantity);
                fullCraftCopy.results.ForEach(result => result.quantity *= craft.quantity);
                craftMerge.ingredients.AddRange(fullCraftCopy.ingredients);
                craftMerge.results.AddRange(fullCraftCopy.results);

                var s = craftMerge.ingredients.Select(e => e);

                craftMerge.moneyCost += fullCraftCopy.moneyCost * craft.quantity;
                craftMerge.timeCost += fullCraftCopy.timeCost * craft.quantity;
            }

            if(user.money < craftMerge.moneyCost)
                {Send(ClientNotification.NotificationData("Activities", "you don't have enough money", 1)); return (false, null);}

            /*We remove items with the same id, and add their quantity together*/
            for(var i = 0; i < craftMerge.ingredients.Count; i++)
            {
                for(var j = i+1; j < craftMerge.ingredients.Count; j++)
                {
                    if(craftMerge.ingredients[i].id == craftMerge.ingredients[j].id)
                    {
                        craftMerge.ingredients[i].quantity += craftMerge.ingredients[j].quantity;
                        craftMerge.ingredients.RemoveAt(j);
                        j--;
                    }
                }
                if(! user.inventory.HasItem(craftMerge.ingredients[i].id, craftMerge.ingredients[i].quantity))
                    {Send(ClientNotification.NotificationData("Activities", "you don't have enough items", 1)); return (false, null);}   
            }

            return (true, craftMerge);
            /*foreach(var ingredient in craftMerge.ingredients)
                user.inventory.RemoveItem(ingredient.id, ingredient.quantity);*/

        }
        protected void SendWaifuToActivity(ClientWebSocketResponse rawData)
        {

            var (user, waifu, validResult) = CheckForActivityValidity(rawData);
            if(!validResult) return;

            var activityRequest = JsonConvert.DeserializeObject<ClientActivityRequest>(rawData.data)!;

            

            Activity activity = new()
            {
                type = activityRequest.activityType,
                waifuID = activityRequest.waifuID!,
            };

            switch(activityRequest.activityType)
            {
                case ActivityType.Cafe or ActivityType.Mining or ActivityType.Exploration:
                    activity.Timeout = Global.baseValues.base_activity_length_in_milliseconds;
                    break;
                case ActivityType.Research:
                    var (validResearchResult, researchNode) = CheckForResearchValidity(user!, rawData.data);
                    if(! validResearchResult) return;
                    activity.Timeout = Activity.GetResearchTimeout(waifu!, researchNode!.cost);
                    activity.researchID = researchNode.id;
                    break;
                case ActivityType.Crafting:
                    var (validCraftResult, craft) = CheckForCraftingValidity(user!, rawData.data);
                    if(! validCraftResult) return;
                    user!.money -= craft!.moneyCost;
                    activity.Timeout = Activity.GetCraftingTimeout(waifu!, craft.timeCost);
                    foreach(var ingredient in craft.ingredients)
                        user.inventory.RemoveItem(ingredient.id, ingredient.quantity);
                    foreach(var result in craft.results)
                        activity.loot.Add(
                            new Loot
                            {
                                lootType = LootType.Item,
                                amount = result.quantity,
                                item = Global.items.Find(item => item.id == result.id)
                            }
                        );
                    
                        
                    break;
                default:
                    Console.Error.WriteLine("ActivityRequest doesn't have a valid type after checking for it????");
                    return;
            }

            waifu!.isDoingSomething = true;
            
            user!.activities.Add(activity);


            var timer = new Activities.ActivityTimer(activity.timeout)
            {
                userId = user.Id,
                activityId = activity.id,
            };
            timer.Start();
            Global.activityTimers.Add(activity.id, timer);
            DBUtils.Update(user);Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
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

            var waifu = user.waifus.Find(waifu => waifu.id == activity.waifuID)!;
            waifu.isDoingSomething = false;
            Loot.GrantLoot(activity.loot, user);
            
            SendLoot([.. activity.loot]);

            user.activities.Remove(activity);
            DBUtils.Update(user);

            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
                data = JsonConvert.SerializeObject(user) 
            }));
        }

        protected void CancelActivity(ClientWebSocketResponse rawData)
        {
            var user = DBUtils.Get<UserData.User>(x => x.Id == rawData.userId);
            if(user is null)
                {Send(ClientNotification.NotificationData("Dungeon", "You can't perform this account with being connected!", 1)); return ;}
            var activity = user.activities.Find(activity => activity.id == Convert.ToUInt64(rawData.data));
            if(activity is null)
                {Send(ClientNotification.NotificationData("Dungeon", "There is no activity with that id", 1)); return ;}
            if(activity.finished)
                {Send(ClientNotification.NotificationData("Dungeon", "This activity is already finished", 1)); return ;}
            user.waifus.Find(waifu => waifu.id == activity.waifuID)!.isDoingSomething = false;
            user.activities.Remove(activity);
            Global.activityTimers[activity.id].Dispose();
            Global.activityTimers.Remove(activity.id);
            DBUtils.Update(user);
            Send(JsonConvert.SerializeObject(new ServerWebSocketResponse
            {
                type = ServerResponseType.ProvideUser,
                data = JsonConvert.SerializeObject(user) 
            }));
        }
    }
}
