using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon.Renovation
{
    public class GarbageRemoveRenovationWorldStep : RenovationWorldStep
    {
        [SerializeField] private RenovationGarbageRemoveStepDataSO _dataSo;
        public RenovationGarbageRemoveStepDataSO DataSo=> _dataSo;
        
        [SerializeField] private GameObject _viewHolder;

        private GameObject _view;

        private bool _isPrepeared;
        
        //TODO add object pool
        public override void PrepareStep()
        {
            base.PrepareStep();
            _isPrepeared = true;
            
            //instantiate view
            if (_view == null)
            {
                _view = Instantiate(_dataSo.Prefab, _viewHolder.transform);
            }
        }

        //TODO need refactoring
        public override void CompleteStep()
        {
            if (_view != null)
            {
                Debug.Log("GarbageRemoveStep completed.");
                base.CompleteStep();
                Destroy(_view);
            }
        }

        public override void ResetStep()
        {
            base.ResetStep();
            _isPrepeared = false;
            
            //instantiate view
            if (_view == null)
            {
                _view = Instantiate(_dataSo.Prefab, _viewHolder.transform);
            }
        }

        public void OnMouseDown()
        {
            if (_isPrepeared)
            {
                CompleteStep();
            }
        }
    }
}