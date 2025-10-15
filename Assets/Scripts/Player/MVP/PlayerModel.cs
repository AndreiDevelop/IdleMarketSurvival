using UniRx;
using Zenject;

namespace RentTycoon
{
    public class PlayerModel
    {
        //TODO add controll of add and subtract currency
        public ReactiveProperty<float> CoinsCount { get; private set; }
        public ReactiveProperty<float> CrystalsCount { get; private set; }

        private PlayerData _playerData;
        private SaveManager _saveManager;
        
        public PlayerModel(SaveManager saveManager)
        {
            _saveManager = saveManager;
            
            _playerData = _saveManager.LoadPlayerData();
            
            CoinsCount = new ReactiveProperty<float>(_playerData.CoinsCount);
            CrystalsCount = new ReactiveProperty<float>(_playerData.CrystalsCount);
        }

        public void AddCurrency(CurrencyType type, float amount)
        {
            switch (type)
            {
                case CurrencyType.Coin:
                    _playerData.CoinsCount += amount;
                    CoinsCount.Value = _playerData.CoinsCount;
                    break;
                case CurrencyType.Crystal:
                    _playerData.CrystalsCount += amount;
                    CrystalsCount.Value = _playerData.CrystalsCount;
                    break;
            }
            
            _saveManager.SavePlayerData(_playerData);
        }

        public void SubtractCurrency(CurrencyType type, float amount)
        {
            switch (type)
            {
                case CurrencyType.Coin:
                    _playerData.CoinsCount -= amount;
                    CoinsCount.Value = _playerData.CoinsCount;
                    break;
                case CurrencyType.Crystal:
                    _playerData.CrystalsCount -= amount;
                    CrystalsCount.Value = _playerData.CrystalsCount;
                    break;
            }
            
            _saveManager.SavePlayerData(_playerData);
        }
    }
}