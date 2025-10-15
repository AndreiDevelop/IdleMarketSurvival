using System;
using RentTycoon.Renovation;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon
{
    //TODO add potential scroll bar
    public class RentZonePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _holder;
        
        [SerializeField] private TextMeshProUGUI _nameType;
        [SerializeField] private Image _iconType;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _icon;

        [SerializeField] private Slider _levelSlider;
        [SerializeField] private TextMeshProUGUI _levelText;

        [SerializeField] private TextMeshProUGUI _levelCoinMultiplier;

        [SerializeField] private TextMeshProUGUI _currentReward;
        [SerializeField] private TextMeshProUGUI _nextReward;

        [SerializeField] private TextMeshProUGUI _timeText;
        
        [SerializeField] private TextMeshProUGUI _upgradeCostText;
        
        [Header("Buttons")]
        [SerializeField] private Button _buttonUpgrade;
        [SerializeField] private Button _buttonClose;
        
        [Inject] private RentZoneModel _rentZoneModel;
        [Inject] private RenovationModel _renovationModel;
        [Inject] private PlayerModel _playerModel;

        private float _upgradeCost;
        
        void Start()
        {
            _rentZoneModel.SelectedRentZone.Subscribe((selected) =>
            {
                if(selected == null)
                {
                    SetActive(false);
                }
                else
                {
                    SetActive(true);
                    Initialize(selected);
                }
            }).AddTo(this);

            //update UI if player coins count changed
            _playerModel.CoinsCount.Subscribe(coinsCount =>
            {
                _buttonUpgrade.interactable = coinsCount >= _upgradeCost;
            }).AddTo(this);
            
            //update UI if upgraded level
            _rentZoneModel.OnUpgradedLevel.Subscribe(rentZone =>
            {
                Initialize(rentZone);
            }).AddTo(this);
        }
        
        private void OnEnable()
        {
            _buttonUpgrade.onClick.AddListener(OnUpgradeButtonClick);
            _buttonClose.onClick.AddListener(OnCloseButtonClick);
        }

        private void OnDisable()
        {
            _buttonUpgrade.onClick.RemoveListener(OnUpgradeButtonClick);
            _buttonClose.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void Initialize(RentZone rentZone)
        {
            _nameType.text = rentZone.TypeData.Name;
            _iconType.sprite = rentZone.TypeData.Icon;
            
            _name.text = rentZone.Data.Name;
            _icon.sprite = rentZone.Data.Icon;
            
            int level = rentZone.SaveData.Level;
            
            LevelMultiplier levelMultiplier = _rentZoneModel.GetLevelMultiplier(level);
            LevelMultiplier prevLvlMultiplier = _rentZoneModel.GetPrevLevelMultiplier(levelMultiplier);
            
            ChangeSliderValue(level, prevLvlMultiplier.LevelMax, levelMultiplier.LevelMax);
            _levelText.text = level + "/" + levelMultiplier.LevelMax;
            
            _levelCoinMultiplier.text = "<sprite=0> x"+levelMultiplier.Multiplier.ToString("0.##");
            
            _currentReward.text = _rentZoneModel.
                GetReward(rentZone.Data.BaseRewardCost, level).ToString("0.##");
            
            _nextReward.text = _rentZoneModel.
                GetReward(rentZone.Data.BaseRewardCost, level + 1).ToString("0.##");
            
            _upgradeCost = _rentZoneModel.
                GetUpgradeCost(rentZone.Data.BaseUpgradeCost, level);
            
            _upgradeCostText.text = "<sprite=0> "+_upgradeCost.ToString("0.##");
            
            _timeText.text = rentZone.Data.BaseRewardGetTimeInSeconds.ToString("0.##");
        }

        private void ChangeSliderValue(int level, int minLevel, int maxLevel)
        {
            _levelSlider.minValue = minLevel;
            _levelSlider.maxValue = maxLevel;
            _levelSlider.value = level;
        }
        
        private void OnRealtorButtonClick()
        {
            throw new NotImplementedException();
        }

        private void OnCloseButtonClick()
        {
            SetActive(false);
        }

        private void OnUpgradeButtonClick()
        {
            _rentZoneModel.UpgradeSelectedRentZone();
        }
        
        private void SetActive(bool isActive)
        {
            _holder.SetActive(isActive);
        }
    }
}

