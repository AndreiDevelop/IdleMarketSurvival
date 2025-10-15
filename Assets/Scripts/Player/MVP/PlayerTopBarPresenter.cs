using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace RentTycoon
{
    public class PlayerTopBarPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsCountText;
        [SerializeField] private TextMeshProUGUI _crystalsCountText;
        
        [Inject] private PlayerModel _playerModel;

        private void Start()
        {
            _playerModel.CoinsCount.Subscribe(coinsCount =>
            {
                _coinsCountText.text = "<sprite=0> "+coinsCount.ToString("0.##");
            }).AddTo(this);
            
            _playerModel.CrystalsCount.Subscribe(crystalsCount =>
            {
                _crystalsCountText.text = "<sprite=0> "+crystalsCount.ToString("0.##");;
            }).AddTo(this);
        }

        public void AddCurrency(CurrencyType type, float amount)
        {
            _playerModel.AddCurrency(type, amount);
        }

        public void SubtractCurrency(CurrencyType type, float amount)
        {
            _playerModel.SubtractCurrency(type, amount);
        }
    }
}