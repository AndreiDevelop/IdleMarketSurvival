using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon.Renovation
{
    public class PropsAddRenovationWorldStep : RenovationWorldStep
    {
        [SerializeField] private Transform _viewHolder;
        [SerializeField] private GameObject _addInicator;

        private bool _isPropsAdded;
        
        private GameObject _addedProps;

        public override void PrepareStep()
        {
            base.PrepareStep();

            if (!_isPropsAdded)
            {
                _addInicator.SetActive(true);
            }
        }

        public override void CompleteStep()
        {
            base.CompleteStep();

            _addInicator.SetActive(false);
        }

        public override void ResetStep()
        {
            base.ResetStep();
            
            _isPropsAdded = false;
            _addInicator.SetActive(false);
            
            DestroyAddedProps();
        }

        public void AddProps(RenovationRentZonePropsAddStepData propsAddStepData)
        {
            if (IsCompleted)
            {
                return;
            }
            
            if (_isPropsAdded)
            {
                return;
            }

            _addedProps = Instantiate(propsAddStepData.PropsAddData.Prefab, _viewHolder);
            _addedProps.transform.position = _viewHolder.position;
            _addedProps.transform.rotation = _viewHolder.rotation;

            _isPropsAdded = true;
            _addInicator.SetActive(false);
        }

        public void RemoveProps()
        {
            if (IsCompleted)
            {
                return;
            }

            DestroyAddedProps();

            _isPropsAdded = false;
            _addInicator.SetActive(true);
        }

        private void DestroyAddedProps()
        {
            if (_addedProps != null)
            {
                Destroy(_addedProps);
                _addedProps = null;
            }
        }
    }
}