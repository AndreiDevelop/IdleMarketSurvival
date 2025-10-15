using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace RentTycoon
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RentZoneWorldView>().FromComponentsInHierarchy().AsTransient();
            
            Container.Bind<RentZonePresenter>().FromComponentsInHierarchy().AsTransient();
        }
    }
}

