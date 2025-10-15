using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace RentTycoon
{
    public class RentZoneUnitPlace
    {
        public ReactiveCommand OnUpgrade = new ReactiveCommand();
        public string Id => Data.Id;
        public RentZoneUnitPlaceData Data { get; protected set; }
        public RentZoneUnitPlaceSaveData SaveData { get; protected internal set; }

        public RentZoneUnitPlace(RentZoneUnitPlaceData data, RentZoneUnitPlaceSaveData saveData)
        {
            Data = data;
            SaveData = saveData;
        }

        public void Upgrade()
        {
            var data = SaveData;
            data.IsUnlocked = true;
            SaveData = data;

            OnUpgrade?.Execute();
        }

        public void AttachUnit(string unitId)
        {
            var data = SaveData;
            data.AttachedUnitId = unitId;
            data.IsUnitAttached = true;
            SaveData = data;
        }

        public void DetachUnit()
        {
            var data = SaveData;
            data.AttachedUnitId = null;
            data.IsUnitAttached = false;
            SaveData = data;
        }

        public void Unlock()
        {
            var data = SaveData;
            data.IsUnlocked = true;
            SaveData = data;
        }
    }
}