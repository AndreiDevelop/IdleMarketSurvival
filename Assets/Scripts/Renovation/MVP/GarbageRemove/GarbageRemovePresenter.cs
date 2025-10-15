using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace RentTycoon.Renovation
{
    public class GarbageRemovePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _holder;
        
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _garbageRemoveText;
        
        [Inject] private RenovationModel _renovationModel;
        
        private Dictionary<RenovationGarbageRemoveStepDataSO, GarbageStruct> _garbageStructs =
            new Dictionary<RenovationGarbageRemoveStepDataSO, GarbageStruct>();

        void Start()
        {
            _renovationModel.GarbageRemoveModel.OnUpdated
                .Subscribe(_ =>
                {
                    UpdateGarbageRemoveText();
                })
                .AddTo(this);
            
            _renovationModel.OnSwitchStepTypes
                .Subscribe(type =>
                {
                    if(type == RenovationStepType.GarbageRemove)
                    {
                        SetActive(true);
                    }
                    else 
                    {
                        SetActive(false);
                    }
                })
                .AddTo(this);
        }

        private void SetActive(bool isActive)
        {
            _holder.SetActive(isActive);
        }
        
        private void UpdateGarbageRemoveText()
        {
            _garbageRemoveText.text = string.Empty;
                    
            bool isAnyGarbageLeft = _renovationModel.WorldSteps.Any(x=>
                x.RenovationStepType == RenovationStepType.GarbageRemove &&
                !x.IsCompleted);

            if (!isAnyGarbageLeft)
            {
                _garbageRemoveText.text = "Completed!";
            }
            else
            {
                _garbageStructs.Clear();
                
                foreach (var step in _renovationModel.WorldSteps)
                {
                    if (step.RenovationStepType == RenovationStepType.GarbageRemove)
                    {
                        var garbageRemoveStep = step as GarbageRemoveRenovationWorldStep;

                        if (garbageRemoveStep == null)
                        {
                            continue;
                        }

                        if (_garbageStructs.ContainsKey(garbageRemoveStep.DataSo))
                        {
                            if (_garbageStructs.TryGetValue(garbageRemoveStep.DataSo, out var garbageStruct))
                            {
                                garbageStruct.GenerallCount++;
                            
                                garbageStruct.CurrentCount = !garbageRemoveStep.IsCompleted ? 
                                    garbageStruct.CurrentCount + 1:
                                    garbageStruct.CurrentCount;
                            
                                _garbageStructs[garbageRemoveStep.DataSo] = garbageStruct;
                            }
                        }
                        else
                        {
                            var garbageStruct = new GarbageStruct
                            {
                                SpriteId = garbageRemoveStep.DataSo.SpriteId,
                                GenerallCount = 1,
                                CurrentCount = !garbageRemoveStep.IsCompleted ? 1 : 0
                            };
                            
                            _garbageStructs.TryAdd(garbageRemoveStep.DataSo, garbageStruct);
                        }
                    }
                }
                
                foreach (var garbage in _garbageStructs)
                {
                    _garbageRemoveText.text += garbage.Value.CurrentCount + "/" + garbage.Value.GenerallCount + 
                                               " " +
                                               "<sprite=" + garbage.Value.SpriteId + ">"+
                                               "\n";
                }
            }
        }
        
        private struct GarbageStruct
        {
            public string SpriteId;
            public int GenerallCount;
            public int CurrentCount;
        }
    }
}