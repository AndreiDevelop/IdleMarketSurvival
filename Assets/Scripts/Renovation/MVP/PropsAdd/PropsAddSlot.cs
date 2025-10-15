using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Unity.VisualScripting;
using Zenject;

namespace RentTycoon.Renovation
{
    public class PropsAddSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public ReactiveProperty<bool> IsClicked { private set; get; } = new ReactiveProperty<bool>(false);
        
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _countText;

        private RenovationModel _renovationModel;
        
        private GameObject _iconObject;
        private Camera _camera;

        //TODO should be fixed and moved to RenovationModel?
        private RenovationRentZonePropsAddStepData _stepData;
        public RenovationRentZonePropsAddStepData StepData => _stepData;
        
        private bool _isTryToAddProps;
        
        //TODO should be fixed and moved to RenovationModel?
        private RenovationWorldStep _renovationWorldStep;

        private int _allCount;
        private int _currentCount;
        
        
        public void Initialize(RenovationModel renovationModel,
            RenovationRentZonePropsAddStepData stepData,
            Camera camera, 
            Sprite icon, 
            int currentCount, 
            int allCount)
        {
            _renovationModel = renovationModel;
            _stepData = stepData;
            _camera = camera;
            _icon.sprite = icon;
            
            _currentCount = currentCount;
            _allCount = allCount;

            UpdateCountText();
        }

        //TODO optimize
        public void OnPointerDown(PointerEventData eventData)
        {
            ClearPrevIconObject();
            
            // Create a new GameObject
            _iconObject = new GameObject("IconObject");

            // Add an Image component to the GameObject
            Image imageComponent = _iconObject.AddComponent<Image>();

            // Set the sprite of the Image component to the current icon
            imageComponent.sprite = _icon.sprite;

            // Set the parent of the new GameObject to the canvas
            _iconObject.transform.SetParent(transform.root, false);

            // Reset the scale of the new GameObject
            _iconObject.transform.localScale = Vector3.one;

            // Adjust the size of the Image to match the icon
            RectTransform rectTransform = _iconObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(_icon.sprite.rect.width, _icon.sprite.rect.height);
            
            IsClicked.Value = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ReleaseClick();
        }
        
        public void ReleaseClick()
        {
            if (_isTryToAddProps)
            {
                _renovationModel.
                    PropsAddModel.
                    DecreaseCountOfStepData(StepData);
                
               _renovationWorldStep.CompleteStep();     
            }
            
            IsClicked.Value = false;
            ClearPrevIconObject();
        }

        private void UpdateCountText()
        {
            _countText.text = _currentCount + "/" + _allCount;
        }
        
        private void ClearPrevIconObject()
        {
            if (_iconObject != null)
            {
                // Destroy the GameObject when the pointer is released
                Destroy(_iconObject);
            }
            
            _isTryToAddProps = false;
            _iconObject = null;
        }
        
        private void Update()
        {
            if (IsClicked.Value && _iconObject != null)
            {
                // Move the iconObject to follow the cursor
                Vector3 mousePosition = Input.mousePosition;
                _iconObject.transform.position = mousePosition;
                UpdateWorldCoordinates();
            }
        }

        public void TryToAddProps(RenovationWorldStep step)
        {
            if (_isTryToAddProps)
            {
                return;
            }

            _currentCount--;
            UpdateCountText();
            
            if(_iconObject != null)
            {
                _iconObject.SetActive(false);
            }
            
            _isTryToAddProps = true;
            _renovationWorldStep = step;
        }

        public void DiscardAddProps()
        {
            if (!_isTryToAddProps)
            {
                return;
            }

            _currentCount++;
            UpdateCountText();
            
            if(_iconObject != null)
            {
                _iconObject.SetActive(true);
            }
            
            _isTryToAddProps = false;
            _renovationWorldStep = null;
        }
        
        private void UpdateWorldCoordinates()
        {
            // take UI icon centre position
            Vector3 iconScreenPos = _iconObject.GetComponent<RectTransform>().position;

            // convert into world coordinates
            Vector3 iconWorldPos = _camera.ScreenToWorldPoint(iconScreenPos);
            // set z to 0 to avoid depth issues
            iconWorldPos.z = 0; 
            
            _renovationModel.
                PropsAddModel.
                DragPropsAdd(this, iconWorldPos);
        }
    }
}