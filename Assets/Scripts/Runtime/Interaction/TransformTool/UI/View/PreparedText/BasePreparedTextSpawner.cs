using TMPro;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.Focus;
using VENTUS.Interaction.TransformTool.UI.Transformer;
using VENTUS.UI;
using VENTUS.UI.TransitionView;

namespace VENTUS.Interaction.TransformTool.UI.View.PreparedText
{
    public abstract class BasePreparedTextSpawner : MonoBehaviour, ISelectionFocusElement
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private PreparedTextView _preparedTextViewPrefab;

        [Header("Positioning")] 
        [SerializeField] private GameObject _positionOriginObject;
        [SerializeField] private BasePositioner _positioner;

        private PreparedTextView _localInstantiatedPrefab;
        private RectTransform _rectTransform;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _inputField.onSelect.AddListener(x => Focus());
        }

        protected virtual void OnDestroy()
        {
            _inputField.onSelect.RemoveListener(x => Focus());
        }

        protected void Focus()
        {
            SelectionFocusSystem.ChangeSelection(typeof(BasePreparedTextSpawner), this);
        }

        protected void Unfocus()
        {
            SelectionFocusSystem.ExitSelection(typeof(BasePreparedTextSpawner));
        }

        public void OnApplying(ISelectionFocusElement previous)
        {
            _localInstantiatedPrefab = Instantiate(_preparedTextViewPrefab, 
                _positioner.GetUpdatedPosition(_positionOriginObject), Quaternion.identity);
            _localInstantiatedPrefab.OnSelectTextElement += SelectTextElement;
        }

        public void OnRestoring()
        {
            _localInstantiatedPrefab.gameObject.Disable(DeactivationType.Destroy);
            _localInstantiatedPrefab.OnSelectTextElement -= SelectTextElement;
        }
        
        private void SelectTextElement(string textToAdd)
        {
            string currentText = _inputField.text;
            int cursorPosition = _inputField.caretPosition;

            currentText = currentText.Insert(cursorPosition, textToAdd);

            _inputField.text = currentText;

            _inputField.caretPosition = cursorPosition + textToAdd.Length;
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _inputField.textComponent.preferredHeight);
        }
    }
}
