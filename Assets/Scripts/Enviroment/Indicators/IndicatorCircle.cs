using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace RentTycoon
{
    public class IndicatorCircle : MonoBehaviour
    {
        public ReactiveCommand OnComplete = new ReactiveCommand();
        
        [SerializeField] private SpriteRenderer _loaderFiller;

        private string _radialFillPropertyName = "_Arc2";
        private int _radialFillValue = 360;
        private int _maxRadialFillValue = 0;
        
        public void StartIndicator(float durationInSeconds)
        {
            _radialFillValue = 360;
            AnimateRadialFill(durationInSeconds).Forget();
        }

        private async UniTaskVoid AnimateRadialFill(float durationInSeconds)
        {
            float elapsedTime = 0f;

            while (elapsedTime < durationInSeconds)
            {
                elapsedTime += Time.deltaTime;
                _radialFillValue = (int)Mathf.Lerp(360, 0, elapsedTime / durationInSeconds);
                _loaderFiller.material.SetFloat(_radialFillPropertyName, _radialFillValue);
                await UniTask.Yield();
            }

            _radialFillValue = 0;
            _loaderFiller.material.SetFloat(_radialFillPropertyName, _radialFillValue);
            OnComplete?.Execute();
        }

        public void ResetIndicator()
        {
            _loaderFiller.material.SetFloat(_radialFillPropertyName, _radialFillValue);
        }
    }
}
