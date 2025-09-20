namespace Nanina.UserData.ItemData
{
    public class UpgradeRequirementItem
    {
        public short item_id;
        public short quantity;
    }

    public class UpgradeRequirement
    {
        public required List<List<UpgradeRequirementItem>> weapon;
        public required List<List<UpgradeRequirementItem>> dress;
        public required List<List<UpgradeRequirementItem>> accessory;
    }
}