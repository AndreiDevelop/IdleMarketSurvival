using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon
{
    public class RentalAdvertisementSlotPresenter : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] protected TextMeshProUGUI _unitNameText;
        [SerializeField] protected TextMeshProUGUI _unitLevelText;
        [SerializeField] protected TextMeshProUGUI _unitRewardIncreasePercentText;
        [SerializeField] protected TextMeshProUGUI _unitUpgradeCostText;
        [SerializeField] protected TextMeshProUGUI _unitBuyCostText;
        
        [Header("Panels")] 
        [SerializeField] private GameObject _lockPanel;
        
        [Header("Icons")]
        [SerializeField] private Image _unitIcon;

        [Header("Buttons")]
        [SerializeField] private Button _buttonOfferRent;
        [SerializeField] private Button _buttonRelocate;
        [SerializeField] private Button _buttonUpgrade;
        [SerializeField] private Button _buttonEndContract;
        [SerializeField] private Button _buttonBuy;
        
        private RentZoneModel _rentZoneModel;
        private RentalModel _rentalModel;
        private RentZoneUnitPlaceModel _rentZoneUnitPlaceModel;
        private PlayerModel _playerModel;
        private Rental _rental;
        private RentZoneUnitPlace _unitPlace;
        
        private float _upgradeCost;
        private float _buyCost;
        
        public void Initialize(
            RentZoneModel rentZoneModel,
            RentalModel rentalModel,
            RentZoneUnitPlaceModel rentZoneUnitPlaceModel,
            PlayerModel playerModel,
            Rental rental,
            RentZoneUnitPlace unitPlace)
        {
            _rentZoneModel = rentZoneModel;
            _rentalModel = rentalModel;
            _rentZoneUnitPlaceModel = rentZoneUnitPlaceModel;
            _playerModel = playerModel;
            _rental = rental;
            _unitPlace = unitPlace;

            Subscribe();

            InitializeBase();

            if (!unitPlace.SaveData.IsUnlocked)
            {
                InitializeLockedPanel();
            }
        }

        private void Subscribe()
        {
            _playerModel.CoinsCount.Subscribe(coinsCount =>
            {
                _buttonUpgrade.interactable = coinsCount >= _upgradeCost;
                _buttonBuy.interactable = coinsCount >= _buyCost;
            }).AddTo(this);
            
            _rentalModel.OnUnlocked.Subscribe((unit) =>
            {
                if (unit.Id == _rental.Id)
                {
                    InitializeBase();
                }
            }).AddTo(this);

            _rentalModel.OnUpgradedLevel.Subscribe((unit) =>
            {
                if (unit.Id == _rental.Id)
                {
                    InitializeBase();
                }
            }).AddTo(this);
            
            _rentZoneUnitPlaceModel.OnUnitAttached.Subscribe(unitPlace =>
            {
                if(unitPlace.SaveData.AttachedUnitId == _rental.Id)
                {
                    InitializeBase();
                }
            }).AddTo(this);
            
            _rentZoneUnitPlaceModel.OnUnitDetached.Subscribe(unitPlace =>
            {
                InitializeBase();
            }).AddTo(this);
        }
        
        private void InitializeBase()
        {
            _lockPanel.SetActive(false);

            int unitLevel = _rental.SaveData.Level;

            _unitIcon.sprite = _rental.Data.Icon;
            _unitNameText.text = "Name: " + _rental.Data.Name;
            _unitLevelText.text = "Level: " + unitLevel;

            _unitRewardIncreasePercentText.text = "Reward: <sprite=0> +"+
                                                  _rental.SaveData.RewardIncreasePercent.ToString("0.##") + "%";
            
            _upgradeCost = _rentZoneModel.
                GetUpgradeCost(_rental.Data.BaseUpgradeCost, unitLevel);
            
            _unitUpgradeCostText.text = "<sprite=0> "+_upgradeCost.ToString("0.##");

            if (_rentZoneUnitPlaceModel.IsRentalAttached(_rental))
            {
                _buttonRelocate.gameObject.SetActive(true);
                _buttonEndContract.gameObject.SetActive(true);
                
                _buttonOfferRent.gameObject.SetActive(false);
            }
            else
            {
                _buttonRelocate.gameObject.SetActive(false);
                _buttonEndContract.gameObject.SetActive(false);
                
                _buttonOfferRent.gameObject.SetActive(true);
            }
        }

        private void InitializeLockedPanel()
        {
            _lockPanel.SetActive(true);
            _unitBuyCostText.text = "<sprite=0> "+_rental.Data.BuyCost.ToString("0.##");
        }
        
        private void OnEnable()
        {
            _buttonOfferRent.onClick.AddListener(OnOfferRentButtonClick);
            _buttonUpgrade.onClick.AddListener(OnUpgradeButtonClick);
            _buttonBuy.onClick.AddListener(OnBuyButtonClick);
            _buttonRelocate.onClick.AddListener(OnRelocateButtonClick);
            _buttonEndContract.onClick.AddListener(OnEndContractButtonClick);
        }

        private void OnDisable()
        {
            _buttonOfferRent.onClick.RemoveListener(OnOfferRentButtonClick);
            _buttonUpgrade.onClick.RemoveListener(OnUpgradeButtonClick);
            _buttonBuy.onClick.RemoveListener(OnBuyButtonClick);
            _buttonRelocate.onClick.RemoveListener(OnRelocateButtonClick);
            _buttonEndContract.onClick.RemoveListener(OnEndContractButtonClick);
        }
        
        #region Buttons

        private void OnBuyButtonClick()
        {
            _rentalModel.UnlockRental(_rental.Id);
        }

        private void OnUpgradeButtonClick()
        {
            _rentalModel.UpgradeRental(_rental.Id);
        }

        private void OnOfferRentButtonClick()
        {
            _rentZoneUnitPlaceModel.AttachUnit(_rental.Id, _unitPlace.Id, _unitPlace.Data.RentZoneId);
        }

        //TODO add switch buttons and check if unit is attached?
        private void OnRelocateButtonClick()
        {
            _rentZoneUnitPlaceModel.AttachUnit(_rental.Id, _unitPlace.Id, _unitPlace.Data.RentZoneId);
        }
        
        private void OnEndContractButtonClick()
        {
            _rentZoneUnitPlaceModel.TryToDetachUnit(_rental.Id);
        }
        
        #endregion
    }
}

