using System;
using TMPro;
using UnityEngine;
using VENTUS.DataStructures;
using VENTUS.DataStructures.Variables;

namespace VENTUS.UI.Input
{
    public class KeyboardManager : MonoBehaviour
    {
        [SerializeField] private CameraVariable _targetCamera;
        [SerializeField] private float _initialDistanceY = -0.5f;
        [SerializeField] private float _initialDistanceZ = 0.5f;
        [SerializeField] private SerializableDictionary<KeyboardTypes, Keyboard> _keyboardTypes = new();

		private Action<string> _onInputWasUpdated;
        private Action<string> _onInputWasSubmitted;
        private Action<int> _onPositionWasUpdated;
        private Action _onInputWasCancelled;
        private TMP_InputField _connectedInputField;

        private Keyboard _activeKeyboard;

        public void GetKeyboardInput(
            KeyboardTypes keyboardType,
            string existingInput = null,
            Action<string> onInputWasUpdated = null, 
            Action<string> onInputWasSubmitted = null,
            Action<int> onPositionWasUpdated = null,
            Action onInputWasCancelled = null,
            TMP_InputField connectedInputField = null)
        {
            _onInputWasUpdated = onInputWasUpdated;
            _onInputWasSubmitted = onInputWasSubmitted;
            _onPositionWasUpdated = onPositionWasUpdated;
            _onInputWasCancelled = onInputWasCancelled;

            if (!_keyboardTypes.TryGetValue(keyboardType, out var keyboard))
            {
                Debug.LogError("The given Keyboard type is not yet defined!");
				onInputWasCancelled?.Invoke();
                return;
			}

            _activeKeyboard = keyboard;

            if (connectedInputField)
            {
                _connectedInputField = connectedInputField;
                _activeKeyboard.SetExistingInput(_connectedInputField.text);
                if (_connectedInputField.TryGetComponent(out InputFieldCaretEvent caretEvent))
                    caretEvent.OnCaretUpdate += SetPosition;
            }
            
            if (existingInput != null)
                _activeKeyboard.SetExistingInput(existingInput);
            
            _activeKeyboard.OnUpdateInput += OnInputWasUpdated;
            _activeKeyboard.OnSubmitInput += OnInputWasSubmitted;
            _activeKeyboard.OnUpdatePosition += OnPositionWasUpdated;
            _activeKeyboard.OnCancelInput += OnInputWasCancelled;
			_activeKeyboard.gameObject.SetActive(true);

            var cameraTransform = _targetCamera.Get().transform;
            _activeKeyboard.transform.parent.position = cameraTransform.position 
                                                 + cameraTransform.forward * _initialDistanceZ 
                                                 + Vector3.up * _initialDistanceY;
        }

        public void CloseKeyboard()
        {
            if (_activeKeyboard == null) return;
            if (_connectedInputField != null)
            {
                if (_connectedInputField.TryGetComponent(out InputFieldCaretEvent caretEvent))
                    caretEvent.OnCaretUpdate -= SetPosition;
                _connectedInputField.ReleaseSelection();
            }
            
            _activeKeyboard.gameObject.SetActive(false);
            _activeKeyboard.OnUpdateInput -= OnInputWasUpdated;
			_activeKeyboard.OnSubmitInput -= OnInputWasSubmitted;
            _activeKeyboard.OnUpdatePosition -= OnPositionWasUpdated;
			_activeKeyboard.OnCancelInput -= OnInputWasCancelled;
            _activeKeyboard.ResetInput();
			_activeKeyboard = null;
            _onInputWasUpdated = null;
            _onInputWasSubmitted = null;
			_onPositionWasUpdated = null;
			_onInputWasCancelled = null;
            _connectedInputField = null;
        }

        public void SetPosition(int caretPosition)
        {
            if (_activeKeyboard == null)
                return;
            
            _activeKeyboard.Position = caretPosition;
        }

        public int GetPosition()
        {
            if (_activeKeyboard == null)
                return -42;
            
            return _activeKeyboard.Position;
        }

        private void OnInputWasUpdated(string input)
        {
            _onInputWasUpdated?.Invoke(input);
            if (_connectedInputField != null)
                _connectedInputField.text = input;
        }

        private void OnPositionWasUpdated(int position)
        {
            _onPositionWasUpdated?.Invoke(position);
            if (_connectedInputField != null)
                _connectedInputField.stringPosition = position;
        }

        private void OnInputWasSubmitted(string input)
        {
            _onInputWasSubmitted?.Invoke(input);
            CloseKeyboard();
        }
        private void OnInputWasCancelled()
        {
            _onInputWasCancelled?.Invoke();
            CloseKeyboard();
		}

        public enum KeyboardTypes
        {
            Numbers,
            Small,
            SmallWithNumbers,
            SmallWithNumbersAndArrows
        }
    }
}
