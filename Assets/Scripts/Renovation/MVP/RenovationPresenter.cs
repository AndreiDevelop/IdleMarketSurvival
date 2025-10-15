using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon.Renovation
{
    public class RenovationPresenter : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private GameObject _holder;

        [Header("Buttons")] 
        [SerializeField] private Button _buttonClose;
        
        [Inject] private RenovationModel _renovationModel;
        [Inject] private RentZoneModel _rentZoneModel;
        
        void Start()
        {
            _renovationModel.OnStartRenovation
                .Subscribe(rentZoneId =>
                {
                    SetActive(true);
                })
                .AddTo(this);
            
            _renovationModel.OnCloseRenovation
                .Subscribe(_ =>
                {
                    SetActive(false);
                })
                .AddTo(this);

            _renovationModel.OnFinishRenovation
                .Subscribe(rentZoneId =>
                {
                    SetActive(false);
                })
                .AddTo(this);
            
            _rentZoneModel.SelectedRentZone
                .Subscribe(rentZone =>
                {
                    if (rentZone!=null && 
                        !string.IsNullOrEmpty(rentZone.Id))
                    {
                        CloseRenovation();
                    }
                })
                .AddTo(this);
        }

        private void OnEnable()
        {
            _buttonClose.onClick.AddListener(CloseRenovation);
        }

        private void OnDisable()
        {
            _buttonClose.onClick.RemoveListener(CloseRenovation);
        }

        private void CloseRenovation()
        {
            _renovationModel.CloseRenovation();
        }
        
        private void SetActive(bool isActive)
        {
            _holder.SetActive(isActive);
        }
    }
}