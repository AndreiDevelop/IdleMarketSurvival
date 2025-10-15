using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RentTycoon.Renovation;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.Serialization;

namespace RentTycoon
{
    public class RentZoneWorldView : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private GameObject _unLockedView;
        [SerializeField] private GameObject _lockedView;
        [SerializeField] private RenovationWorldView _renovationWorldView;
        
        [Header("Buttons")]
        [SerializeField] private ButtonCollider _buttonSelectRentZone;
        [SerializeField] private ButtonCurrency _buttonGetRentReward;
        [SerializeField] private ButtonCurrency _buttonBuyRentZone;
        [SerializeField] private ButtonCurrency _buttonRenovateRentZone;

        [Header("Indicators")] 
        [SerializeField] private IndicatorCircle _indicatorRentReward; 
        
        [Inject] private RentZoneModel _rentZoneModel;
        [Inject] private RenovationModel _renovationModel;
        [Inject] private PlayerModel _playerModel;

        private RentZone _rentZone;

        private CancellationTokenSource _cancellationUnlockView = new CancellationTokenSource();
        private CancellationTokenSource _cancellationLockView = new CancellationTokenSource();
        
        //TODO add change of reward after upgrade
        private void Start()
        {
            _rentZone = _rentZoneModel.GetRentZone(_id);

            _rentZoneModel.SelectedRentZone.Subscribe(rentZone =>
            {
                if (rentZone == null)
                {
                    return;
                }
                
                SetActiveUnLockedView(_rentZone.SaveData.IsUnlocked);
            }).AddTo(this);

            
            _renovationModel.OnCloseRenovation
                .Subscribe(_ =>
                {
                    SetActiveUnLockedView(_rentZone.SaveData.IsUnlocked);
                })
                .AddTo(this);
            /*
            _renovationModel.OnFinishRenovation
                .Subscribe(rentZoneId =>
                {
                    if (rentZoneId == _id)
                    {
                        SetActiveUnLockedView(_rentZone.SaveData.IsUnlocked);
                    }
                })
                .AddTo(this);
                */
            
            _renovationWorldView.gameObject.SetActive(true);
            SetActiveUnLockedView(_rentZone.SaveData.IsUnlocked);
        }

        private void InitializeUnlockView()
        {
            if (_cancellationUnlockView.Token.CanBeCanceled)
            {
                _cancellationUnlockView?.Cancel();
            }
            
            _cancellationUnlockView?.Dispose();
            _cancellationUnlockView = new CancellationTokenSource();
            
            _buttonGetRentReward.OnClick.Subscribe(_=>
            {
                CollectRewardRentZone(_rentZone.SaveData.RewardCount);
            }).AddTo(_cancellationUnlockView.Token);
            
            _rentZone.OnUpdate.Subscribe(rewardCount =>
            {
                UpdateRentReward(rewardCount);
            }).AddTo(_cancellationUnlockView.Token);
            
            _rentZone.OnUpgrade.Subscribe(rewardCount =>
            {
                UpdateRentReward(rewardCount);
            }).AddTo(_cancellationUnlockView.Token);
            
            _buttonSelectRentZone.OnClick.Subscribe(_=>
            {
                SelectRentZone();
            }).AddTo(_cancellationUnlockView.Token);

            StartUpdateWithDelay();
        }

        private void InitializeLockView()
        {
            if(_cancellationLockView.Token.CanBeCanceled)
            {
                _cancellationLockView?.Cancel();
            }

            _cancellationLockView?.Dispose();
            _cancellationLockView = new CancellationTokenSource();
            
            _buttonBuyRentZone.UpdateCurrency(_rentZone.Data.BuyCost);

            _playerModel.CoinsCount.Subscribe(cointCount =>
            {
                if (cointCount >= _rentZone.Data.BuyCost)
                {
                    _buttonBuyRentZone.SetInteractable(true);
                }
                else
                {
                    _buttonBuyRentZone.SetInteractable(false);
                }
            }).AddTo(_cancellationLockView.Token);
            
            _buttonBuyRentZone.OnClick.Subscribe(_=>
            {
                _rentZoneModel.UnlockRentZone(_id);
                SetActiveUnLockedView(true);
                
                //select this rent zone
                _rentZoneModel.SelectRentZone(_id);
                
            }).AddTo(_cancellationLockView.Token);
            
            _buttonRenovateRentZone.OnClick.Subscribe(_=>
            {
                _lockedView.SetActive(false);
                _renovationWorldView.gameObject.SetActive(true);
                _renovationWorldView.StartRenovation(_id);
            }).AddTo(_cancellationLockView.Token);
        }
        
        private void SetActiveUnLockedView(bool isActive)
        {
            _unLockedView.SetActive(isActive);
            _lockedView.SetActive(!isActive);

            if (isActive)
            {
                InitializeUnlockView();
            }
            else
            {
                InitializeLockView();
            }
        }
        
        private void CollectRewardRentZone(float rewardCount)
        {
            //swith to this rentZone if some rent zone selected
            if (_rentZoneModel.SelectedRentZone.Value != null
                && _rentZoneModel.SelectedRentZone.Value.Id != _id)
            {
                _rentZoneModel.SelectRentZone(_id);
            }
            
            _playerModel.AddCurrency(CurrencyType.Coin, rewardCount);
            _buttonGetRentReward.gameObject.SetActive(false);

            StartUpdateRentZoneReward();
        }

        private async UniTaskVoid StartUpdateWithDelay()
        {
            await UniTask.Delay(1000);
            StartUpdateRentZoneReward();
        }
        
        private void StartUpdateRentZoneReward()
        {
            _indicatorRentReward.StartIndicator(_rentZone.Data.BaseRewardGetTimeInSeconds);
            _rentZone.UpdateRentReward();
        }
        
        private void UpdateRentReward(float rewardCount)
        {
            _buttonGetRentReward.UpdateCurrency(rewardCount);
            _buttonGetRentReward.gameObject.SetActive(true);
        }
        
        private void SelectRentZone()
        {
            _rentZoneModel.SelectRentZone(_id);
        }
    }
}

