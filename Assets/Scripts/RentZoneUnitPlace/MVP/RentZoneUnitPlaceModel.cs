using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace RentTycoon
{
    public class RentZoneUnitPlaceModel
    {
        public ReactiveCommand<RentZoneUnitPlace> OnUnlocked = new ReactiveCommand<RentZoneUnitPlace>();
        public ReactiveCommand<RentZoneUnitPlace> OnSelected = new ReactiveCommand<RentZoneUnitPlace>();
        public ReactiveCommand<RentZoneUnitPlace> OnUnitAttached = new ReactiveCommand<RentZoneUnitPlace>();
        public ReactiveCommand<RentZoneUnitPlace> OnUnitDetached = new ReactiveCommand<RentZoneUnitPlace>();
        
        private List<RentZoneUnitPlace> _rentZoneUnitPlaces;

        private PlayerModel _playerModel;
        private RentalModel _rentalModel;

        private SaveManager _saveManager;

        public RentZoneUnitPlaceModel(PlayerModel playerModel,
            RentalModel rentalModel,
            SaveManager saveManager,
            List<RentZoneUnitPlaceData> rentZoneUnitPlaceData)
        {
            _saveManager = saveManager;
            _playerModel = playerModel;
            _rentalModel = rentalModel;
            
            List<RentZoneUnitPlaceSaveData> rentZoneUnitPlaceSaveData = _saveManager
                .LoadRentZoneUnitPlaceData();

            _rentZoneUnitPlaces = new List<RentZoneUnitPlace>(rentZoneUnitPlaceData.Count);
            
            for (int i = 0; i < rentZoneUnitPlaceData.Count; i++)
            {
                RentZoneUnitPlaceSaveData rentZoneUnitPlaceSaveDataItem = 
                    rentZoneUnitPlaceSaveData
                        .Find(x => x.Id == rentZoneUnitPlaceData[i].Id && 
                                   x.RentZoneId == rentZoneUnitPlaceData[i].RentZoneId);
                
                _rentZoneUnitPlaces.Add( 
                    new RentZoneUnitPlace(
                        rentZoneUnitPlaceData[i], 
                        rentZoneUnitPlaceSaveDataItem));
            }
        }

        public void TryToDetachUnit(string unitId)
        {
            RentZoneUnitPlace attachedUnitPlace = null;

            foreach (var unitPlace in _rentZoneUnitPlaces)
            {
                if(unitPlace.SaveData.AttachedUnitId == unitId)
                {
                    attachedUnitPlace = unitPlace;
                    break;
                }
            }

            if (attachedUnitPlace == null)
            {
                Debug.LogError($"Unit place with id {unitId} not found");
                return;
            }
            
            attachedUnitPlace.DetachUnit();
            OnUnitDetached?.Execute(attachedUnitPlace);
        }
        
        public void AttachUnit(string unitId, string unitPlaceId, string rentZoneId)
        {
            TryToDetachUnit(unitId);
            
            RentZoneUnitPlace unitPlace = GetUnitPlace(unitPlaceId, rentZoneId);

            if (unitPlace == null)
            {
                Debug.LogError($"Unit place with id {unitPlaceId} not found");
                return;
            }

            unitPlace.AttachUnit(unitId);
            OnUnitAttached?.Execute(unitPlace);
        }

        public void UnlockUnitPlace(string id, string rentZoneId)
        {
            RentZoneUnitPlace unitPlace = GetUnitPlace(id, rentZoneId);

            if (unitPlace == null)
            {
                Debug.LogError($"Unit place with id {id} not found");
                return;
            }

            unitPlace.Unlock();
            _playerModel.SubtractCurrency(CurrencyType.Coin, unitPlace.Data.BuyCost);
            OnUnlocked?.Execute(unitPlace);
        }

        public void UpgradeUnit(string id, string rentZoneId)
        {
            RentZoneUnitPlace unitPlace = GetUnitPlace(id, rentZoneId);

            if (unitPlace == null)
            {
                Debug.LogError($"Unit place with id {id} not found");
                return;
            }

            _rentalModel.UpgradeRental(unitPlace.SaveData.AttachedUnitId);
        }

        public Rental GetUnit(string id, string rentZoneId)
        {
            RentZoneUnitPlace unitPlace = GetUnitPlace(id, rentZoneId);

            if (unitPlace == null)
            {
                Debug.LogError($"Unit place with id {id} not found");
                return null;
            }

            return _rentalModel.GetRental(unitPlace.SaveData.AttachedUnitId);
        }

        public void Select(string id, string rentZoneId)
        {
            RentZoneUnitPlace unitPlace = GetUnitPlace(id, rentZoneId);

            if (unitPlace == null)
            {
                Debug.LogError($"Unit place with id {id} not found");
                return;
            }

            OnSelected?.Execute(unitPlace);
        }

        public bool IsRentalAttached(Rental rental)
        {
            return _rentZoneUnitPlaces.Exists(x=>x.SaveData.AttachedUnitId == rental.Id);
        }
        
        public RentZoneUnitPlace GetUnitPlace(string id, string rentZoneId)
        {
            return _rentZoneUnitPlaces
                .Find(x => x.Id == id && x.Data.RentZoneId == rentZoneId);
        }
    }
}