using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace RentTycoon
{
    public class RentalModel
    {
        public ReactiveCommand<Rental> OnUpgradedLevel = new ReactiveCommand<Rental>();
        public ReactiveCommand<Rental> OnUnlocked = new ReactiveCommand<Rental>();
        
        public ReactiveProperty<Rental> SelectedRental { get; private set; }
        public ReactiveCollection<Rental> Rentals;

        private List<RentalData> _RentalsData;
        private UpgradeData _upgradeData;
        private SaveManager _saveManager;
        private PlayerModel _playerModel;

        public RentalModel(
            List<RentalData> RentalsData,
            UpgradeData upgradeData,
            SaveManager saveManager,
            PlayerModel playerModel)
        {
            _RentalsData = RentalsData;
            _upgradeData = upgradeData;
            _saveManager = saveManager;
            _playerModel = playerModel;
            
            InitializeRentals();

            SelectedRental = new ReactiveProperty<Rental>();
        }

        private void InitializeRentals()
        {
            Rentals = new ReactiveCollection<Rental>();

            for (int i = 0; i < _RentalsData.Count; i++)
            {
                RentalSaveData saveData = _saveManager
                    .LoadRentalData()
                    .Find(x => x.Id == _RentalsData[i].Id);

                Rentals.Add(new Rental(
                    _RentalsData[i],
                    saveData));
            }
        }

        public Rental GetRental(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("Rental id is null or empty");
                return null;
            }
            
            bool isRentalsEmpty = Rentals.IsEmpty();

            if (isRentalsEmpty)
            {
                Debug.LogError("Rentals are empty");
            }

            return isRentalsEmpty ? null : Rentals.First(x => x.Id.Equals(id));
        }

        public void SelectRental(string id)
        {
            SelectedRental.Value = GetRental(id);
        }

        public void UpgradeSelectedRental(int upgradeLevelNumbers = 1)
        {
            if (SelectedRental.Value == null)
            {
                Debug.LogError("SelectedRental is null. Please select a rent unit before upgrading.");
                return;
            }

            int newLevel = SelectedRental.Value.SaveData.Level + upgradeLevelNumbers;
            float newPercent = GetReward(SelectedRental.Value.Data.RentRewardIncreasePercent, newLevel);

            SelectedRental.Value.Upgrade(newPercent, newLevel);
            _saveManager.SaveRentalData(SelectedRental.Value);

            OnUpgradedLevel.Execute(SelectedRental.Value);
        }

        public void UpgradeRental(string unitId, int upgradeLevelNumbers = 1)
        {
            Rental rental = GetRental(unitId);
            if (rental == null)
            {
                Debug.LogError("SelectedRental is null. Please select a rent unit before upgrading.");
                return;
            }

            int newLevel = rental.SaveData.Level + upgradeLevelNumbers;
            float newPercent = GetReward(rental.Data.RentRewardIncreasePercent, newLevel);
            float upgradeCost = GetUpgradeCost(rental.Data.BaseUpgradeCost, newLevel);

            rental.Upgrade(newPercent, newLevel);
            _saveManager.SaveRentalData(rental);
            _playerModel.SubtractCurrency(CurrencyType.Coin, (int)upgradeCost);
            
            OnUpgradedLevel.Execute(rental);
        }
        
        public void UnlockRental(string id)
        {
            Rental Rental = GetRental(id);
            Rental.Unlock();
            _saveManager.SaveRentalData(Rental);
            
            OnUnlocked?.Execute(Rental);
        }

        public LevelCost GetLevelUpgradeCost()
        {
            int currentLevel = SelectedRental.Value.SaveData.Level;
            float generalUpgradeCost = GetUpgradeCost(SelectedRental.Value.Data.BaseUpgradeCost, currentLevel);

            LevelCost levelCost = new LevelCost()
            {
                CountOfLevels = 1,
                MaxLevel = currentLevel,
                Cost = generalUpgradeCost
            };

            return levelCost;
        }

        public LevelCost GetMaxLevelUpgradeCost()
        {
            int currentLevel = SelectedRental.Value.SaveData.Level;
            int maxLevel = _upgradeData.LevelMultipliers.Last().LevelMax;
            float generalUpgradeCost = GetUpgradeCost(SelectedRental.Value.Data.BaseUpgradeCost, currentLevel);

            LevelCost levelCost = new LevelCost()
            {
                CountOfLevels = 1,
                MaxLevel = currentLevel,
                Cost = generalUpgradeCost
            };

            if (currentLevel <= maxLevel - 1)
            {
                for (int i = currentLevel + 1; i < maxLevel; i++)
                {
                    float currentUpgradeCost = GetUpgradeCost(SelectedRental.Value.Data.BaseUpgradeCost, i);

                    generalUpgradeCost += currentUpgradeCost;

                    if (_playerModel.CoinsCount.Value >= generalUpgradeCost)
                    {
                        levelCost.CountOfLevels++;
                        levelCost.MaxLevel = i;
                        levelCost.Cost = generalUpgradeCost;
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