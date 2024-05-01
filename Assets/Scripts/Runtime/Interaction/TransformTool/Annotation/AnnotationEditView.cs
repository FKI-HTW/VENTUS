using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Controlling;
using VENTUS.DataStructures.CommandLogic.Snapshot;
using VENTUS.UI.Input;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    public class AnnotationEditView : AnnotationView
    {
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private Button _confirmButton;
        
        private KeyboardManager _keyboardManager;
        private RectTransform _titleRect;
        private RectTransform _descriptionRect;

        private AnnotationDataCommandSnapshot _previousSnapshot;

        protected void Awake()
        {
            _keyboardManager = GameObject.FindGameObjectWithTag("KeyboardManager").GetComponent<KeyboardManager>();
            
            if (_keyboardManager == null) return;
            _titleInput.onSelect.AddListener(x => InitTitleInputKeyboard());
            _descriptionInput.onSelect.AddListener(x => InitDescriptionInputKeyboard());
            _titleRect = _titleInput.GetComponent<RectTransform>();
            _descriptionRect = _descriptionInput.GetComponent<RectTransform>();
        }

        protected override void InternalInitialize()
        {
            UpdateContent();
            
            _confirmButton.onClick.AddListener(UpdateAnnotationData);
        }
        
        public override void OnValuesChanged()
        {
            UpdateContent();
        }

        private void UpdateContent()
        {
            _titleInput.text = annotationController.Title;
            _titleRect.sizeDelta = new Vector2(_titleRect.sizeDelta.x, _titleInput.textComponent.preferredHeight);

            _descriptionInput.text = annotationController.Description;
            _descriptionRect.sizeDelta = new Vector2(_descriptionRect.sizeDelta.x, _descriptionInput.textComponent.preferredHeight);
        }

        private void UpdateAnnotationData()
        {
            _keyboardManager.CloseKeyboard();
            
            if (_titleInput.text == annotationController.Title &&
                _descriptionInput.text == annotationController.Description)
                return;
            
            annotationController.UpdateValuesServer(InstanceFinder.ClientManager.Connection, _titleInput.text, _descriptionInput.text);
        }
        
        private void InitTitleInputKeyboard()
        {
            if (SceneManager.IsInXRMode)
            {
                _keyboardManager.GetKeyboardInput(
                    KeyboardManager.KeyboardTypes.Small,
                    connectedInputField: _titleInput,
                    onInputWasUpdated: _ => _titleRect.sizeDelta = new(
                        _titleRect.sizeDelta.x, 
                        _titleInput.textComponent.preferredHeight
                    )
                );
            }
        }

        private void InitDescriptionInputKeyboard()
        {
            if (SceneManager.IsInXRMode)
            {
                _keyboardManager.GetKeyboardInput(
                    KeyboardManager.KeyboardTypes.Small,
                    connectedInputField: _descriptionInput,
                    onInputWasUpdated: _ => _descriptionRect.sizeDelta = new(
                        _descriptionRect.sizeDelta.x, 
                        _descriptionInput.textComponent.preferredHeight
                    )
                );
            }
        }
    }
}
