using UnityEngine;

namespace RentTycoon
{
    [System.Serializable]
    public struct RentalData
    {
        public string Name;
        public string Id;
        public Sprite Icon;
        public float BuyCost;
        public float BaseUpgradeCost;
        public int RentRewardIncreasePercent;
    }
}
