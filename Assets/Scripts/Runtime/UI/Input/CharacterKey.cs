using System;
using TMPro;
using UnityEngine;

namespace VENTUS.UI.Input
{
    public class CharacterKey : MonoBehaviour
    {
        [SerializeField] private Keyboard _keyboard;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _normalValue;
        [SerializeField] private string _shiftValue;
    
        private bool _isShifted;

        public event Action<string> OnKey;
        public event Action<string> OnKeyUp;
        public event Action<string> OnKeyDown;

        private void Awake()
        {
            if (_text == null)
                _text = transform.GetChild(0).GetComponent<TMP_Text>();

            if (_keyboard == null)
                return;

            _keyboard.OnShifted += ShiftKey;
            OnKey += _keyboard.PressCharacterKey;
        }

        public void PressKey() => OnKey?.Invoke(_isShifted ? _shiftValue : _normalValue);
		public void PressKeyUp() => OnKeyUp?.Invoke(_isShifted ? _shiftValue : _normalValue);
        public void PressKeyDown() => OnKeyDown?.Invoke(_isShifted ? _shiftValue : _normalValue);
        public void ShiftKey() => ShiftKey(!_isShifted);
        public void ShiftKey(bool isShifted)
        {
            _isShifted = isShifted;
            _text.text = isShifted ? _shiftValue : _normalValue;
	    }
    }
}
