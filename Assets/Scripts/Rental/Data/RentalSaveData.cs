using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [System.Serializable]
    public struct RentalSaveData
    {
        public string Id;
        public float RewardIncreasePercent;
        public bool IsUnlocked;
        public int Level;
    }
}
