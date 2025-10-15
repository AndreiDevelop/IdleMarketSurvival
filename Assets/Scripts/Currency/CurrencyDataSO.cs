using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "CurrencyDataSO", menuName = "Data/SO/new CurrencyDataSO")]
    public class CurrencyDataSO : ScriptableObject
    {
        public CurrencyData currency;
        
        [SerializeField] private TMP_SpriteAsset _spriteAsset;
        public TMP_SpriteAsset SpriteAsset => _spriteAsset;
    }
}

