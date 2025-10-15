using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace RentTycoon.Renovation
{
    public class PropsAddModel
    {
        public ReactiveCommand OnUpdated = new ReactiveCommand();
        
        public List<PropsAddRenovationWorldStep> WorldSteps { get; private set; }
            = new List<PropsAddRenovationWorldStep>();

        public List<RenovationRentZoneData> InitData { get; private set; } 
            = new List<RenovationRentZoneData>();

        //create renovation SaveData
        //should have RentZoneId
        //renovation Id
        //count of left renovations
        
        private float _snapDistance = 0.3f; 
        private string _rentZoneId;

        public void TakeProps()
        {
            
        }

        public void ReturnProps()
        {
            
        }
        
        public PropsAddModel(RenovationRentZonesSO renovationRentZonesSo)
        {
            InitData = renovationRentZonesSo.Data;
        }

        public void StartRenovation(string rentZoneId)
        {
            _rentZoneId = rentZoneId;
        }
        
        public void TryToAddPropsStep(RenovationWorldStep step)
        {
            if(step.RenovationStepType == RenovationStepType.PropsAdd)
            {
                var propsAddStep = step as PropsAddRenovationWorldStep;
                if (propsAddStep != null)
                {
                    WorldSteps.Add(propsAddStep);
                }
            }
        }
        
        //TODO add optimisation
        public void DragPropsAdd(PropsAddSlot slot, Vector3 propsPosition)
        {
            float minDist = Mathf.Infinity;

            foreach (var step in WorldSteps)
            {
                //check if step is completed or props added
                if (step.IsCompleted)
                {
                    continue;
                }
                
                float dist = Vector3.Distance(propsPosition, step.WorldPosition);

                if (dist <= _snapDistance)
                {
                    slot.TryToAddProps(step);
                    step.AddProps(slot.StepData);
                    
                    break;
                }
                else
                {
                    slot.DiscardAddProps();
                    step.RemoveProps();
                }
            }
        }
        
        public void Updated()
        {
            OnUpdated?.Execute();
        }
        
        public List<RenovationRentZonePropsAddStepData> GetCurrentStepsData()
        {
            return InitData
                .Find(x => x.RentZoneId == _rentZoneId)
                .RenovationStep;
        }
        
        public void DecreaseCountOfStepData(RenovationRentZonePropsAddStepData data)
        {
            var step = GetCurrentStepsData()
                .FirstOrDefault(x => x.Id == data.Id);

            //step.Count--;
        }
        
        public void IncreaseCountOfStepData(RenovationRentZonePropsAddStepData data)
        {
            var step = GetCurrentStepsData()
                .FirstOrDefault(x => x.Id == data.Id);

            //step.Count++;
        }
    }
}