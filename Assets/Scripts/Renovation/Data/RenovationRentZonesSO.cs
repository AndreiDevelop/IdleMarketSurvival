using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon.Renovation
{
    [System.Serializable]
    public struct RenovationRentZonePropsAddStepData
    {
        [SerializeField] private string _id;
        [SerializeField] private RenovationPropsAddStepDataSO _propsAddData;
        [SerializeField] private int _count;
        
        public string Id => _id;
        public RenovationPropsAddStepDataSO PropsAddData => _propsAddData;
        public int Count=> _count;
    }

    [System.Serializable]
    public struct RenovationRentZoneData
    {
        [SerializeField] private string _rentZoneId;
        public List<RenovationRentZonePropsAddStepData> RenovationStep;
        
        public string RentZoneId => _rentZoneId;
    }

    [CreateAssetMenu(fileName = "RenovationRentZonesSO", menuName = "Data/SO/Renovation/new RenovationRentZonesSO")]
    public class RenovationRentZonesSO : ScriptableObject
    {
        public List<RenovationRentZoneData> Data;
    }
}