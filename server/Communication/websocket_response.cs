
namespace Nanina.Communication
{
    /*
        The format which the Client uses to send information to the server
    */
    public enum ClientResponseType
    {
        UpdateTheme,
        UpdateOsuId,
        UpdateLocale,
        UpdatePreferedGame,
        UpdateWaifuDB,
        UpdateItemDB,
        UpdateSetDB,
        StartFight,
        ClaimFight,
        ClaimDungeonFight,
        GetSession,
        AddBeatmap,
        ConnectWithDiscord,
        GetPullResults,
        GetMapData,
        VerifyOsuId,
        VerifyMaimaiToken,
        StartDungeon,
        StopDungeon,
        EquipItem,
        UnequipItem,
        Logout,
        GetLevelRewards,
        GetLevelRewardsData,
        SendWaifuToActivity,
        UseUserConsumable,
        CheckContinuousFight,
        UpgradeEquipment,
        UpdateUserWaifu,
        UpdateUserInventory,
        BecomeAdmin,
        ClaimActivity,
        CancelActivity,
    }
    public class ClientWebSocketResponse
    {
        public required ClientResponseType type;
        public required string data;
        public required string sessionId;
        public required string userId;
    }

    /*
        The format which the Server uses to send back information to the client
    */

    public enum ServerResponseType
    {
        ProvideUser,
        ProvideMapData,
        ConfirmFightClaim,
        ProvidePullResults,
        Notification,
        ProvideSession,
        ProvideBanners,
        ProvideDungeons,
        ProvideResearchNodes,
        ProvideCraftingRecipes,
        ProvideLoot,
        ProvideAndGiveLoot,
        ProvideWaifuDB,
        ProvideSetDB,
        ProvideEquipmentDB,
        ProvideItemDB,
        ProvideMaimaiChartData,
        ProvideLevelRewardsData,
        ProvideActiveDungeon,
        OsuIdUpdateSuccess,
        MaimaiTokenUpdateSuccess,
        ProvideContinuousFightResults,
        ConfirmEquip,
        ConfirmUnequip,
        ConfirmActivity,
        ConfirmCancelActivity,
        ConfirmActivityClaim,
        ConfirmDungeonStarted,
        FreeWaifus,
        ConfirmLvlRewardClaim,
        GiveXPToWaifu,
        ConfirmUserConsumableConsumption,
        UpdateEnergy,
        

    }
    public class ServerWebSocketResponse
    {
        public required ServerResponseType type;
        public required string data;
    }
}
