using RentTycoon.Renovation;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace RentTycoon
{
    public class SceneLoadingInstaller : MonoInstaller
    {
        [Header("SO")]
        [SerializeField] private RentZonesDataSO _rentZonesDataSo;
        [SerializeField] private RentZoneTypesDataSO rentZoneTypesDataSo;
        [SerializeField] private RentUpgradeDataSO _rentUpgradeDataSo;
        [SerializeField] private RentZoneUnitPlaceDataSO _rentZoneUnitPlaceDataSo;
        [SerializeField] private RentalsDataSO _RentalsDataSo;
        [SerializeField] private PlayerDataSO _playerDataSO;
        [SerializeField] private RenovationRentZonesSO _renovationRentZonesSo;
        
        [Header("Managers")]
        [SerializeField] private SceneLoadingManager _sceneLoadingManager;
        
        public override void InstallBindings()
        {
            Container.Bind<RentZonesDataSO>().
                FromInstance(_rentZonesDataSo).
                AsSingle();
            
            Container.Bind<RentUpgradeDataSO>().
                FromInstance(_rentUpgradeDataSo).
                AsSingle();
          
            Container.Bind<SceneLoadingManager>().
                FromComponentInHierarchy().
                AsSingle();

            var saveManager = new SaveManager();
            Container.Bind<SaveManager>().
                FromInstance(saveManager).
                AsSingle();

            var playerModel = new PlayerModel(saveManager);
            Container.Bind<PlayerModel>().
                FromInstance(playerModel).
                AsSingle();
            
            var rentalModel = new RentalModel(_RentalsDataSo.Data, 
                _rentUpgradeDataSo.Data, 
                saveManager,
                playerModel);
            Container.Bind<RentalModel>()
                .FromInstance(rentalModel)
                .AsSingle();
            
            var rentZoneModel = new RentZoneModel(
                _rentZonesDataSo.Data, 
                rentZoneTypesDataSo.Data, 
                _rentUpgradeDataSo.Data,
                saveManager,
                playerModel);
            Container.Bind<RentZoneModel>()
                .FromInstance(rentZoneModel)
                .AsSingle();
            
            var rentalPlaceModel = new RentZoneUnitPlaceModel(
                playerModel,
                rentalModel,
                saveManager, 
                _rentZoneUnitPlaceDataSo.Data);
            Container.Bind<RentZoneUnitPlaceModel>()
                .FromInstance(rentalPlaceModel)
                .AsSingle();

            var renovationModel = new RenovationModel(rentZoneModel, _renovationRentZonesSo);
            Container.Bind<RenovationModel>()
                .FromInstance(renovationModel)
                .AsSingle();
        }
    }
}