using UnityEngine;

namespace RentTycoon
{
    [System.Serializable]
    public enum RentZoneType
    {
        Houses = 0,
        СommercialRealEstate = 1,
        Environment = 2,
    }
    
    [System.Serializable]
    public struct RentZoneTypeData
    {
        public string Name;
        public RentZoneType Type;
        public Sprite Icon;
    }
}
