using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [System.Serializable]
    public struct RentZoneData
    {
        public string Name;
        public string Id;
        public Sprite Icon;
        
        public RentZoneType Type;

        public CurrencyType CurrencyType;
        public float BuyCost;
        
        public float BaseRewardCost;
        public float BaseUpgradeCost;
        public float BaseRewardGetTimeInSeconds;
        
        
    }
}

