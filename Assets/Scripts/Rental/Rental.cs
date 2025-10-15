using UniRx;
using UnityEngine;

namespace RentTycoon
{
    public class Rental
    {
        public ReactiveCommand<float> OnUpgrade = new ReactiveCommand<float>();
        
        public string Id => Data.Id;
        public RentalData Data { get; protected set; }
        public RentalSaveData SaveData { get; protected internal set; }
        
        public Rental(RentalData data, RentalSaveData saveData)
        {
            Data = data;
            SaveData = saveData;
        }
        
        public void Upgrade(float newRewardCount, int newLevel = 1)
        {
            var data = SaveData;
            data.Level = newLevel; 
            data.RewardIncreasePercent = newRewardCount;
            SaveData = data;
            
            OnUpgrade?.Execute(newRewardCount);
        }
        
        public void Unlock()
        {
            var data = SaveData;
            data.IsUnlocked = true;
            SaveData = data;
        }
    }
}
