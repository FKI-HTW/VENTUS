using System;
using UnityEngine;

namespace VENTUS.UI.Input
{
	public class SpecialKey : MonoBehaviour
	{
		[SerializeField] private Keyboard _keyboard;
		[SerializeField] private SpecialKeyType _keyType;

		public event Action<SpecialKeyType> OnKey;
		public event Action<SpecialKeyType> OnKeyUp;
		public event Action<SpecialKeyType> OnKeyDown;

		private void Awake()
		{
			if (_keyboard == null)
				return;

			OnKey += _keyboard.PressSpecialKey;
		}

		public void PressKey() => OnKey?.Invoke(_keyType);
		public void PressKeyUp() => OnKeyUp?.Invoke(_keyType);
		public void PressKeyDown() => OnKeyDown?.Invoke(_keyType);
	}

	public enum SpecialKeyType
	{
		Submit,
		Spacebar,
		Enter,
		Backspace,
		Delete,
		DeleteAll,
		LeftArrow,
		RightArrow,
		Shift,
		Tab
	}
}
