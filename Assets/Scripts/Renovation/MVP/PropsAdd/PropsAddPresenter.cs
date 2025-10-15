using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RentTycoon.Renovation
{
    public class PropsAddPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _holder;
        [SerializeField] private PropsAddSlot _propsAddSlotPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _content;
        
        [Inject] private RenovationModel _renovationModel;

        private List<PropsAddSlot> _propsAddSlots = new List<PropsAddSlot>();
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Camera _camera;
        
        void Start()
        {
            _camera = Camera.main;
            
            _renovationModel.OnSwitchStepTypes
                .Subscribe(type =>
                {
                    if(type == RenovationStepType.PropsAdd)
                    {
                        InitializePropsAdd();
                        SetActive(true);
                    }
                    else 
                    {
                        SetActive(false);
                    }
                })
                .AddTo(this);

            _renovationModel.
                PropsAddModel.
                OnUpdated
                .Subscribe(_ =>
                {
                    InitializePropsAdd();
                })
                .AddTo(this);;
        }

        private void SetActive(bool isActive)
        {
            _holder.SetActive(isActive);
        }

        private void InitializePropsAdd()
        {
            ClearContent();
            
            foreach (var stepData in _renovationModel.
                         PropsAddModel.
                         GetCurrentStepsData())
            {
                var propsSlot = Instantiate(_propsAddSlotPrefab, _content);
                
                propsSlot.Initialize(
                    _renovationModel,
                    stepData,
                    _camera,
                    stepData.PropsAddData.Sprite, 
                    stepData.Count,
                    stepData.Count);
                
                _propsAddSlots.Add(propsSlot);
                
                _propsAddSlots.Last().IsClicked.Subscribe(isClicked =>
                {
                    _scrollRect.enabled = !isClicked;
                }).AddTo(_cancellationTokenSource.Token);
            }
        }

        //TODO optimize add objects pool
        private void ClearContent()
        {
            if(_cancellationTokenSource != null && 
               _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource?.Cancel();
            }
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            
            foreach (var propsSlot in _propsAddSlots)
            {
                Destroy(propsSlot.gameObject);
            }
            
            _propsAddSlots.Clear();
        }
    }
}