using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "RentZoneTypesDataSO", menuName = "Data/SO/new RentZoneTypesDataSO")]
    public class RentZoneTypesDataSO : ScriptableObject
    {
        public List<RentZoneTypeData> Data;
    }
}
