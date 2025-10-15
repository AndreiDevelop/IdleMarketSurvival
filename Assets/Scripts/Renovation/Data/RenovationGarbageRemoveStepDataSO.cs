using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon.Renovation
{
    [CreateAssetMenu(fileName = "RenovationGarbageRemoveStepDataSO", menuName = "Data/SO/Renovation/new RenovationGarbageRemoveStepDataSO")]
    public class RenovationGarbageRemoveStepDataSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _spriteId;
        [SerializeField] private GameObject _prefab;

        public string Name => _name;
        public string SpriteId => _spriteId;
        public GameObject Prefab => _prefab;
    }
}

