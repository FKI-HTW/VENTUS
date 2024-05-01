using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VENTUS.UI
{
    public class ColorTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _targetGraphic;
        [SerializeField] private float _fadeDuration;
    
        [Header("Normal Color")]
        [SerializeField] private Color32 _normalColor;
        [SerializeField] private Color32 _normalHoverColor;
        [SerializeField] private Color32 _normalPressedColor;
        
        [Header("Selectable Color")]
        [SerializeField] private Selectable _targetSelectable;
        [SerializeField] private Color _disabledColor;
    
        private bool _isPointerDown;
        private bool _isPointerContained;
        private Color32 _currentPointerColor;
        private bool _currentlyInteractable;

        private void Awake()
        {
            if (_targetGraphic == null)
            {
                Debug.LogError($"_targetGraphic is not set in the inspector. Please apply it inside the object {name}!");
            }
            
            _currentPointerColor = _normalColor;
            SetCurrentPointerColor();
        }

        private void Start()
        {
            if (_targetSelectable != null)
            {
                _currentlyInteractable = _targetSelectable.interactable;
                UpdateDisabledColor();
            }
        }

        private void OnDisable()
        {
            SetDefaultColor();
        }

        private void OnEnable()
        {
            SetDefaultColor();
        }

        private void Update()
        {
            if (_targetSelectable == null || _currentlyInteractable == _targetSelectable.interactable)
                return;

            _currentlyInteractable = _targetSelectable.interactable;
            UpdateDisabledColor();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerContained = true;
            
            _currentPointerColor = _normalHoverColor;
            SetCurrentPointerColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerContained = false;
        
            if (_isPointerDown) 
                return;

            SetDefaultColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
            _currentPointerColor = _normalPressedColor;
            SetCurrentPointerColor();
        }
    
        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;

            if (!_isPointerContained)
                SetDefaultColor();
        }
        
        private void UpdateDisabledColor()
        {
            if (!_targetSelectable.interactable)
            {
                _targetGraphic.DOColor(_disabledColor, _fadeDuration).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                _targetGraphic.DOColor(_currentPointerColor, _fadeDuration).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
        }
        
        private void SetDefaultColor()
        {
            _currentPointerColor = _normalColor;
            SetCurrentPointerColor();
        }

        private void SetCurrentPointerColor()
        {
            if (!_currentlyInteractable)
                return;
            
            DOTween.Kill(_targetGraphic);
            _targetGraphic.DOColor(_currentPointerColor, _fadeDuration).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
    }
}
