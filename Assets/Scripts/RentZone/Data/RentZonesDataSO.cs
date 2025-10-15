using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "RentZoneDataSO", menuName = "Data/SO/new RentZoneDataSO")]
    public class RentZonesDataSO : ScriptableObject
    {
        public List<RentZoneData> Data;
    }
}

