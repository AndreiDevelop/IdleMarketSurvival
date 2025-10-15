using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace RentTycoon.Renovation
{
    //TODO break on PropsAdd and GarbageRemove
    public class RenovationModel
    {
        public ReactiveCommand<RenovationStepType> OnSwitchStepTypes = new ReactiveCommand<RenovationStepType>();

        public ReactiveCommand<string> OnStartRenovation = new ReactiveCommand<string>();
        public ReactiveCommand OnCloseRenovation = new ReactiveCommand();
        
        public ReactiveCommand<string> OnFinishRenovation = new ReactiveCommand<string>();

        public List<RenovationWorldStep> WorldSteps { get; private set; } = new List<RenovationWorldStep>();

        private string _rentZoneId;
        
        private RentZoneModel _rentZoneModel;

        private GarbageRemoveModel _garbageRemoveModel;
        public GarbageRemoveModel GarbageRemoveModel => _garbageRemoveModel;
        
        private PropsAddModel _propsAddModel;
        public PropsAddModel PropsAddModel => _propsAddModel;
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public RenovationModel(RentZoneModel rentZoneModel, RenovationRentZonesSO renovationRentZonesSo)
        {
            _rentZoneModel = rentZoneModel;
            
            _garbageRemoveModel = new GarbageRemoveModel();
            _propsAddModel = new PropsAddModel(renovationRentZonesSo);
        }

        //TODO need refactoring
        public void StartRenovation(string rentZoneId, List<RenovationWorldStep> renovationSteps)
        {
            CancellToken();

            //deactivate selected rent zone
            _rentZoneModel.SelectRentZone(string.Empty);
            
            _rentZoneId = rentZoneId;
            _propsAddModel.StartRenovation(_rentZoneId);
            
            WorldSteps.AddRange(renovationSteps);
            
            foreach(var step in WorldSteps)
            {
                _propsAddModel.TryToAddPropsStep(step);

                step.OnCompleted
                    .Subscribe(renovationStep =>
                    {
                        if (AllStepsIsCompleted())
                        {
                            FinishRenovation();
                        }
                        else
                        {
                            HandleStepComplete(renovationStep);
                        }
                    })
                    .AddTo(_cancellationTokenSource.Token);

                step.ResetStep();
            }

            //update garbage collect UI
            if (!AllGarbageRemoveStepsIsCompleted())
            {
                OnSwitchStepTypes?.Execute(RenovationStepType.GarbageRemove);
                _garbageRemoveModel.Updated();
            }
            
            OnStartRenovation?.Execute(_rentZoneId);
        }

        private void HandleStepComplete(RenovationWorldStep worldStep)
        {
            //copleted last garbage collection step
            if (AllGarbageRemoveStepsIsCompleted() && 
                worldStep.RenovationStepType == RenovationStepType.GarbageRemove)
            {
                _garbageRemoveModel.Updated();
                _propsAddModel.Updated();

                PrepareAllStepsByType(RenovationStepType.PropsAdd);
                
                OnSwitchStepTypes?.Execute(RenovationStepType.PropsAdd);
            }
            //complete garbage collection step
            else if(worldStep.RenovationStepType == RenovationStepType.GarbageRemove)
            {
                _garbageRemoveModel.Updated();
            }
            //complete props add step
            else if(worldStep.RenovationStepType == RenovationStepType.PropsAdd)
            {
                _propsAddModel.Updated();
            }
        }
        
        public void CloseRenovation()
        {
            CancellToken();
            
            OnCloseRenovation.Execute();

            WorldSteps.Clear();
            
            _rentZoneId = string.Empty;
        }
        
        public void FinishRenovation()
        {
            CancellToken();
            
            OnFinishRenovation.Execute(_rentZoneId);

            WorldSteps.Clear();
            _rentZoneId = string.Empty;
        }

        private void PrepareAllStepsByType(RenovationStepType stepType)
        {
            foreach (var step in WorldSteps)
            {
                if (step.RenovationStepType == stepType)
                {
                    step.PrepareStep();
                }
            }
        }

        private bool AllGarbageRemoveStepsIsCompleted()
        {
            foreach (var step in WorldSteps)
            {
                if (step.RenovationStepType == RenovationStepType.GarbageRemove
                    && !step.IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        private bool AllStepsIsCompleted()
        {
            foreach (var step in WorldSteps)
            {
                if (!step.IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        private void CancellToken()
        {
            if (_cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource?.Cancel();
            }
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}