using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon
{
    public class RentalAdvertisementPresenter : MonoBehaviour
    {
        [SerializeField] private RentalAdvertisementSlotPresenter RentalAdvertisementSlotPrefab;
        [SerializeField] private GameObject _holder;
        [SerializeField] private Transform _content;
        [SerializeField] private Button _buttonClose;
        
        [Inject] private RentZoneModel _rentZoneModel;
        [Inject] private RentalModel _RentalModel;
        [Inject] private RentZoneUnitPlaceModel _rentZoneUnitPlaceModel;
        [Inject] private PlayerModel _playerModel;
        
        private List<GameObject> _slots = new List<GameObject>();
        
        private void Start()
        {
            _rentZoneUnitPlaceModel.OnSelected.Subscribe((unitPlace) =>
            {
                if (unitPlace != null)
                {
                    _holder.SetActive(true);
                    Initialize(unitPlace);
                }
            }).AddTo(this);
            
            _rentZoneUnitPlaceModel.OnUnitAttached.Subscribe((unitPlace) =>
            {
                if (unitPlace != null)
                {
                    _holder.SetActive(false);
                }
            }).AddTo(this);
        }

        //TODO: add pool of objects
        private void Initialize(RentZoneUnitPlace unitPlace)
        {
            ClearSlots();
            
            foreach (var Rental in _RentalModel.Rentals)
            {
                var slot = Instantiate(RentalAdvertisementSlotPrefab, _content);
                
                slot.Initialize(
                    _rentZoneModel,
                    _RentalModel,
                    _rentZoneUnitPlaceModel,
                    _playerModel,
                    Rental,
                    unitPlace);
                
                _slots.Add(slot.gameObject);
            }
        }

        private void ClearSlots()
        {
            if (_slots == null || _slots.Count == 0)
            {
                return;
            }

            foreach (var slot in _slots)
            {
                Destroy(slot.gameObject);
            }
            
            _slots.Clear();
        }
        
        private void OnEnable()
        {
            _buttonClose.onClick.AddListener(OnCloseButtonClick);
        }

        private void OnDisable()
        {
            _buttonClose.onClick.RemoveListener(OnCloseButtonClick);
        }
        
        private void OnCloseButtonClick()
        {
            _holder.SetActive(false);
        }
    }
}

