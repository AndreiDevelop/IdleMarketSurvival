using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace RentTycoon
{
    public class RentZoneManager : MonoBehaviour
    {
        [SerializeField] 
        private RentZone[] _rentZones;
    }
}

