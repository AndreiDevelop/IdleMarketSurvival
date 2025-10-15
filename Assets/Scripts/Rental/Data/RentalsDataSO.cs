using System.Collections.Generic;
using RentTycoon;
using UnityEngine;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "RentalsDataSO", menuName = "Data/SO/new RentalsDataSO")]
    public class RentalsDataSO : ScriptableObject
    {
        public List<RentalData> Data;
    }
}
