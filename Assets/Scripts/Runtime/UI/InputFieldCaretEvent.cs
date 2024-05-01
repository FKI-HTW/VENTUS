using System;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

namespace VENTUS.UI
{
	[RequireComponent(typeof(TMP_InputField))]
	public class InputFieldCaretEvent : MonoBehaviour, IPointerDownHandler
	{
		private TMP_InputField _inputField;
		private int _caretPosition;
		
		public event Action<int> OnCaretUpdate;

		private void Awake()
		{
			_inputField ??= GetComponent<TMP_InputField>();
			_caretPosition = _inputField.stringPosition;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_inputField.stringPosition == _caretPosition) return;
			_caretPosition = _inputField.stringPosition;
			OnCaretUpdate?.Invoke(_caretPosition);
		}
	}
}
