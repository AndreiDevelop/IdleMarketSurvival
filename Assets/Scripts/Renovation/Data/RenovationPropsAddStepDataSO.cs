using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon.Renovation
{
    [CreateAssetMenu(fileName = "RenovationPropsAddStepDataSO", menuName = "Data/SO/Renovation/new RenovationPropsAddStepDataSO")]
    public class RenovationPropsAddStepDataSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _prefab;
        
        public string Name => _name;
        public Sprite Sprite => _sprite;
        public GameObject Prefab => _prefab;
    }
}