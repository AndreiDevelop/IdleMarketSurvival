using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    public class ButtonCurrency : MonoBehaviour
    {
        public ReactiveCommand OnClick = new ReactiveCommand();

        [SerializeField] private bool _isShowCurrencyIcon;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private ButtonCollider _buttonCollider;
        [SerializeField] private TextMeshPro _currencyText;

        private Color _defaultColor;
        private IDisposable _subscription;
        
        private void Awake()
        {
            _defaultColor = _spriteRenderer.color;
            SetInteractable(true);
        }
        
        public void UpdateCurrency(float currencyCount)
        {
            _currencyText.text = _isShowCurrencyIcon
                ? "<sprite=0> " + currencyCount.ToString("0.##")
                : currencyCount.ToString("0.##");
        }

        public void SetInteractable(bool isInteractable)
        {
            if (isInteractable)
            {
                if (_subscription != null)
                {
                    _subscription?.Dispose();
                }
                
                _subscription = _buttonCollider.OnClick.Subscribe(_=>
                {
                    OnClick?.Execute();
                }).AddTo(this);
                
                _spriteRenderer.color = _defaultColor;
            }
            else
            {
                _subscription?.Dispose();
                _subscription = null;

                _spriteRenderer.color = Color.gray;
            }
        }
    }
}

