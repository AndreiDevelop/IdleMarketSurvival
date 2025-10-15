using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using Zenject;

namespace RentTycoon
{
    public class RentZone
    {
        public ReactiveCommand<float> OnUpdate = new ReactiveCommand<float>();
        public ReactiveCommand<float> OnUpgrade = new ReactiveCommand<float>();
        public string Id => Data.Id;
        public RentZoneData Data { get; protected set; }
        public RentZoneTypeData TypeData { get; protected set; }
        public RentZoneSaveData SaveData { get; protected internal set; }
        
        public RentZone(RentZoneData data, 
            RentZoneTypeData typeData, 
            RentZoneSaveData saveData)
        {
            Data = data;
            TypeData = typeData;
            SaveData = saveData;
        }

        public void Upgrade(float newRewardCount, int newLevel = 1)
        {
            var data = SaveData;
            data.Level = newLevel;
            data.RewardCount = newRewardCount;
            SaveData = data;
            
            OnUpgrade?.Execute(newRewardCount);
        }
        
        public void Unlock()
        {
            var data = SaveData;
            data.IsUnlocked = true;
            SaveData = data;
        }
        
        //TODO add cancellation token
        public async UniTask UpdateRentReward()
        {
            await UniTask.Delay((int)Data.BaseRewardGetTimeInSeconds * 1000);
            OnUpdate?.Execute(SaveData.RewardCount);
        }
    }
}

