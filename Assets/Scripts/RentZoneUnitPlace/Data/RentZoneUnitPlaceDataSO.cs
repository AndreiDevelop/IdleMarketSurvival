using System.Collections.Generic;
using UnityEngine;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "RentZoneUnitPlaceDataSO", menuName = "Data/SO/new RentZoneUnitPlaceDataSO")]
    public class RentZoneUnitPlaceDataSO : ScriptableObject
    {
        public List<RentZoneUnitPlaceData> Data;
    }
}