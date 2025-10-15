using UnityEngine;

namespace RentTycoon
{
    [System.Serializable]
    public enum CurrencyType
    {
        Coin = 0,
        Crystal = 1
    }
    
    [System.Serializable]
    public struct CurrencyData
    {
        public CurrencyType Type;
        public string Name;
        
        [HideInInspector] 
        public int Count;
    }
}

