using System;
using UnityEngine;
using UniRx;

namespace RentTycoon.Renovation
{
    public abstract class RenovationWorldStep : MonoBehaviour
    {
        [SerializeField] private RenovationStepType _renovationStepType;

        public RenovationStepType RenovationStepType => _renovationStepType;
        
        public ReactiveCommand<RenovationStepType> OnPrepared = new ReactiveCommand<RenovationStepType>();
        public ReactiveCommand<RenovationWorldStep> OnCompleted = new ReactiveCommand<RenovationWorldStep>();
        
        protected bool _isCompleted;
        public bool IsCompleted=> _isCompleted;
        
        private Vector3 _worldPosition;
        public Vector3 WorldPosition=> _worldPosition;

        private void Awake()
        {
            _worldPosition = gameObject.transform.position;
        }

        public virtual void PrepareStep()
        {
            OnPrepared?.Execute(_renovationStepType);
        }

        public virtual void CompleteStep()
        {
            _isCompleted = true;
            OnCompleted?.Execute(this);
        }
        
        public virtual void ResetStep()
        {
            _isCompleted = false;
        }
    }
}
