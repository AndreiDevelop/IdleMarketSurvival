using UnityEngine;

namespace RentTycoon
{
    [CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Data/SO/new PlayerDataSO")]
    public class PlayerDataSO : ScriptableObject
    {
        [SerializeField] private string _playerName;
        [SerializeField] private Sprite _playerIcon;
        
        public string PlayerName => _playerName;
        public Sprite PlayerIcon => _playerIcon;
    }
}