using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon
{
    public class RentZoneUnitPlacePresenter : MonoBehaviour
    {
        [SerializeField] private string _id;

        [Header("Icons")] 
        [SerializeField] private Image _addNewUnitIcon;
        [SerializeField] private Image _unitIcon;
        
        [Header("Texts")]
        [SerializeField] protected TextMeshProUGUI _unitNameText;
        //[SerializeField] protected TextMeshProUGUI _unitLevelText;
        [SerializeField] protected TextMeshProUGUI _unitRewardIncreasePercentText;
        [SerializeField] protected TextMeshProUGUI _unitUpgradeCostText;
        [SerializeField] protected TextMeshProUGUI _unitPlaceBuyCostText;
        
        [Header("Panels")] 
        [SerializeField] private GameObject _lockPanel;
        [SerializeField] private GameObject _unlockPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button _buttonSelect;
        [SerializeField] private Button _buttonUpgrade;
        [SerializeField] private Button _buttonBuy;
        
        [Inject] private RentZoneModel _rentZoneModel;
        [Inject] private RentalModel _rentalModel;
        [Inject] private RentZoneUnitPlaceModel _rentZoneUnitPlaceModel;
        [Inject] private PlayerModel _playerModel;
        
        private string _rentZoneId;
        private float _upgradeCost;
        private float _buyCost;

        private RentZoneUnitPlace _unitPlace;
        private Rental _rental;
        
        void Start()
        {
            _rentZoneModel.SelectedRentZone.Subscribe((selected) =>
            {
                if(selected != null)
                {
                    _rentZoneId = selected.Id;
                    Initialize();
                }
            }).AddTo(this);

            _playerModel.CoinsCount.Subscribe(coinsCount =>
            {
                _buttonUpgrade.interactable = coinsCount >= _upgradeCost;
                _buttonBuy.interactable = coinsCount >= _buyCost;
            }).AddTo(this);
            
            _rentZoneUnitPlaceModel.OnUnitAttached.Subscribe(unitPlace =>
            {
                if (IsCurRentalPlace(unitPlace))
                {
                    InitializeUnlock();
                }
            }).AddTo(this);
            
            _rentZoneUnitPlaceModel.OnUnitDetached.Subscribe(unitPlace =>
            {
                if (IsCurRentalPlace(unitPlace))
                {
                    InitializeUnlock();
                }
            }).AddTo(this);
            
            _rentalModel.OnUpgradedLevel.Subscribe(rental =>
            {
                if (rental.Id == _unitPlace.SaveData.AttachedUnitId)
                {
                    InitializeUnitAttached();
                }
            }).AddTo(this);
        }

        private void OnEnable()
        {
            _buttonSelect.onClick.AddListener(OnSelectButtonClick);
            _buttonUpgrade.onClick.AddListener(OnUpgradeButtonClick);
            _buttonBuy.onClick.AddListener(OnBuyButtonClick);
        }
        
        private void OnDisable()
        {
            _buttonSelect.onClick.RemoveListener(OnSelectButtonClick);
            _buttonUpgrade.onClick.RemoveListener(OnUpgradeButtonClick);
            _buttonBuy.onClick.RemoveListener(OnBuyButtonClick);
        }

        private bool IsCurRentalPlace(RentZoneUnitPlace unitPlace)
        {
            return unitPlace != null &&
                   !string.IsNullOrEmpty(_rentZoneId) &&
                   _unitPlace != null &&
                   unitPlace.Data.RentZoneId == _rentZoneId &&
                   unitPlace.Id == _unitPlace.Id;

        }
        
        private void Initialize()
        {
            _unitPlace = _rentZoneUnitPlaceModel.GetUnitPlace(_id, _rentZoneId);

            if (_unitPlace.SaveData.IsUnlocked)
            {
                InitializeUnlock();
            }
            else
            {
                InitializeLock();
            }
        }

        private void InitializeUnlock()
        {
            _lockPanel.SetActive(false);
            _unlockPanel.SetActive(true);
            
            if (_unitPlace.SaveData.IsUnitAttached)
            {
                InitializeUnitAttached();
            }
            else
            {
                _buttonUpgrade.gameObject.SetActive(false);
                _unitIcon.gameObject.SetActive(false);
                _addNewUnitIcon.gameObject.SetActive(true);
            }
        }

        private void InitializeUnitAttached()
        {
            _addNewUnitIcon.gameObject.SetActive(false);

            _rental = _rentalModel.GetRental(_unitPlace.SaveData.AttachedUnitId);
            
            int unitLevel = _rental.SaveData.Level;

            _unitIcon.sprite = _rental.Data.Icon;
            
            _unitNameText.text = _rental.Data.Name;
            //_unitLevelText.text = "Lvl:" + unitLevel;

            _unitRewardIncreasePercentText.text = "Rwd:<sprite=0>+"+
                                                  _rental.SaveData.RewardIncreasePercent.ToString("0.#") + "%";
            
            _upgradeCost = _rentZoneModel.
                GetUpgradeCost(_rental.Data.BaseUpgradeCost, unitLevel);
            
            _unitUpgradeCostText.text = "<sprite=0> "+_upgradeCost.ToString("0.##");
            _buttonUpgrade.gameObject.SetActive(true);
            
            _unitIcon.gameObject.SetActive(true);
        }
        
        private void InitializeLock()
        {
            _buyCost = _unitPlace.Data.BuyCost;
            
            _unitPlaceBuyCostText.text = "<sprite=0> "+_buyCost.ToString("0.##");
            
            _lockPanel.SetActive(true);
            _unlockPanel.SetActive(false);
        }

        #region Buttons

        private void OnBuyButtonClick()
        {
            _rentZoneUnitPlaceModel.UnlockUnitPlace(_id, _rentZoneId);
            _unitPlace = _rentZoneUnitPlaceModel.GetUnitPlace(_id, _rentZoneId);
            
            InitializeUnlock();
        }

        private void OnUpgradeButtonClick()
        {
            _rentZoneUnitPlaceModel.UpgradeUnit(_id, _rentZoneId);
        }

        private void OnSelectButtonClick()
        {
            _rentZoneUnitPlaceModel.Select(_id, _rentZoneId);
        }

        #endregion
    }
}