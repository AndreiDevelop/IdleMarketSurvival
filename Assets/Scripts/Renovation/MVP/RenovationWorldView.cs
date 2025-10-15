using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace RentTycoon.Renovation
{
    public class RenovationWorldView : MonoBehaviour
    {
        [SerializeField] private List<RenovationWorldStep> _renovationSteps;
        
        [Inject] private RenovationModel _renovationModel;
        
        public void Start()
        {
            //prepare garbage steps on start
            _renovationModel.OnStartRenovation
                .Subscribe(_ =>
                {
                    PrepareGarbageSteps();
                })
                .AddTo(this);

            PrepareGarbageSteps();
            
            //reset all steps on close renovation
            _renovationModel.OnCloseRenovation
                .Subscribe(_ =>
                {
                    foreach (var renovation in _renovationSteps)
                    {
                        renovation.ResetStep();
                    }
                })
                .AddTo(this);
        }
        
        private void PrepareGarbageSteps()
        {
            foreach (var renovation in _renovationSteps)
            {
                if (renovation.RenovationStepType == RenovationStepType.GarbageRemove)
                {
                    renovation.PrepareStep();
                }
            }
        }
        
        public void StartRenovation(string rentZoneId)
        {
            _renovationModel.StartRenovation(rentZoneId, _renovationSteps);
        }
    }
}
