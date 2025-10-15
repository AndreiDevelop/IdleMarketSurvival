using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using NUnit.Framework;
using UnityEngine;
using UniRx;
using Zenject;

namespace RentTycoon
{
    public class RentZoneModel
    {
        public ReactiveCommand<RentZone> OnUpgradedLevel = new ReactiveCommand<RentZone>();
        
        public ReactiveProperty<RentZone> SelectedRentZone { get; private set; }
        public ReactiveCollection<RentZone> RentZones;
        
        private List<RentZoneData> _rentZonesData;
        private List<RentZoneTypeData> _rentZoneTypesData;
        private UpgradeData _upgradeData;

        private SaveManager _saveManager;
        
        private PlayerModel _playerModel;
        
        public RentZoneModel(
            List<RentZoneData> rentZonesData, 
            List<RentZoneTypeData> rentZoneTypesData,
            UpgradeData upgradeData,
            SaveManager saveManager,
            PlayerModel playerModel)
        {
            _rentZonesData = rentZonesData;
            _rentZoneTypesData = rentZoneTypesData;
            _upgradeData = upgradeData;
            _saveManager = saveManager;
            _playerModel = playerModel;
            
            InitializeRentZones();
            
            SelectedRentZone = new ReactiveProperty<RentZone>();
        }

        private void InitializeRentZones()
        {
            RentZones = new ReactiveCollection<RentZone>();

            for(int i = 0; i < _rentZonesData.Count; i++)
            {
                RentZoneTypeData rentZoneTypeData = _rentZoneTypesData.
                    Find(x => 
                        x.Type == _rentZonesData[i].Type);

                RentZoneSaveData saveData = _saveManager.
                    LoadRentZoneData().
                    Find(x=>
                        x.Id == _rentZonesData[i].Id);

                RentZones.Add(new RentZone(
                    _rentZonesData[i], 
                    rentZoneTypeData,
                    saveData));
            }
        }
        
        public RentZone GetRentZone(string id)
        {
            bool isRentZonesEmpty = RentZones.IsEmpty();

            if (isRentZonesEmpty)
            {
                Debug.LogError("RentZones are empty");
            }
            
            return isRentZonesEmpty || string.IsNullOrEmpty(id) ? null : RentZones.First(x=>x.Id.Equals(id));
        }
        
        public void SelectRentZone(string id)
        {
            var rentZone = GetRentZone(id);
            
            //Force notify to update UI
            if (SelectedRentZone.Value == rentZone)
            {
                SelectedRentZone.SetValueAndForceNotify(rentZone);
            }
            else
            {
                SelectedRentZone.Value = rentZone;
            }
        }
        
        public void UpgradeSelectedRentZone(int upgradeLevelNumbers = 1)
        {
            if (SelectedRentZone.Value == null)
            {
                Debug.LogError("SelectedRentZone is null. Please select a rent zone before upgrading.");
                return;
            }

            int newLevel = SelectedRentZone.Value.SaveData.Level + upgradeLevelNumbers;
            float newReward = GetReward(SelectedRentZone.Value.Data.BaseRewardCost, newLevel); 
            float upgradeCost = GetUpgradeCost(SelectedRentZone.Value.Data.BaseUpgradeCost, newLevel);
            
            SelectedRentZone.Value.Upgrade(newReward, newLevel);
            _saveManager.SaveRentZoneData(SelectedRentZone.Value);
            _playerModel.SubtractCurrency(CurrencyType.Coin, (int)upgradeCost);
            
            OnUpgradedLevel.Execute(SelectedRentZone.Value);
        }

        public void UnlockRentZone(string id)
        {
            RentZone rentZone = GetRentZone(id);
            rentZone.Unlock();
            _saveManager.SaveRentZoneData(rentZone);
            _playerModel.SubtractCurrency(CurrencyType.Coin, rentZone.Data.BuyCost);
        }

        public LevelCost GetLevelUpgradeCost()
        {
            int currentLevel = SelectedRentZone.Value.SaveData.Level;
            float generallUpgradeCost = GetUpgradeCost(SelectedRentZone.Value.Data.BaseUpgradeCost, currentLevel);

            LevelCost levelCost = new LevelCost()
            {
                CountOfLevels = 1,
                MaxLevel = currentLevel,
                Cost = generallUpgradeCost
            };

            return levelCost;
        }
        
        //for multiple upgrade
        public LevelCost GetMaxLevelUpgradeCost()
        {
            int currentLevel = SelectedRentZone.Value.SaveData.Level;
            int maxLevel = _upgradeData.LevelMultipliers.Last().LevelMax;
            float generallUpgradeCost = GetUpgradeCost(SelectedRentZone.Value.Data.BaseUpgradeCost, currentLevel);

            LevelCost levelCost = new LevelCost()
            {
                CountOfLevels = 1,
                MaxLevel = currentLevel,
                Cost = generallUpgradeCost
            };
            
            if (currentLevel <= maxLevel - 1)
            {
                for (int i = currentLevel + 1; i < maxLevel; i++)
                {
                    float currentUpgradeCost = GetUpgradeCost(SelectedRentZone.Value.Data.BaseUpgradeCost, i);

                    //increase generall upgradeCost
                    generallUpgradeCost += currentUpgradeCost;

                    //check if player has enough coins
                    if (_playerModel.CoinsCount.Value >= generallUpgradeCost)
                    {
                        levelCost.CountOfLevels++;
                        levelCost.MaxLevel = i;
                        levelCost.Cost = generallUpgradeCost;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return levelCost;
        }

        public float GetUpgradeCost(float upgradeCost, int level)
        {
            return (float)(upgradeCost * Math.Pow(Constants.UpgradeMultiplier, level));
        }

        public float GetReward(float rewardCost, int level)
        {
            float reward = (float)(rewardCost * Math.Pow(Constants.RewardMultiplier, level));
            
            float levelMultiplier = GetLevelMultiplier(level).Multiplier;
            reward *= levelMultiplier;

            return reward;
        }
        
        public LevelMultiplier GetLevelMultiplier(int level)
        {
            return _upgradeData.GetLevelMultiplier(level);
        }
        
        public LevelMultiplier GetPrevLevelMultiplier(LevelMultiplier currentLvlMultiplier)
        {
            return _upgradeData.GetPrevLevelMultiplier(currentLvlMultiplier);
        }
    }
}

