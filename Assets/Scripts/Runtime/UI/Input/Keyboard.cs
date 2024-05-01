using System;
using UnityEngine;

namespace VENTUS.UI.Input
{
	public class Keyboard : MonoBehaviour
	{
		[SerializeField] private string _input;
		public string Input
		{
			get => _input;
		}

		[SerializeField] private int _position;
		public int Position
		{
			get => _position;
			set
			{
				if (_position == value) return;

				_position = Mathf.Clamp(value, 0, _input.Length);
				OnUpdatePosition?.Invoke(_position);
			}
		}

		[SerializeField] private bool _isShifted;
		public bool IsShifted
		{
			get => _isShifted;
			set
			{
				if (_isShifted == value) return;

				_isShifted = value;
				OnShifted?.Invoke(value);
			}
		}

		public event Action<bool> OnShifted;
		public event Action<string> OnUpdateInput;
		public event Action<string> OnSubmitInput;
		public event Action OnCancelInput;
		public event Action<int> OnUpdatePosition;

		private void OnEnable()
		{
			OnShifted?.Invoke(_isShifted);
		}

		public void SetExistingInput(string input)
		{
			_input = input;
			Position = _input.Length;
		}

		public void SubmitInput()
		{
			OnSubmitInput?.Invoke(_input);
		}

		public void CancelInput()
		{
			OnCancelInput?.Invoke();
		}

		public void ResetInput()
		{
			_input = string.Empty;
			_position = 0;
		}

		public void PressCharacterKey(string character)
		{
			_input = _input.Insert(_position, character);
			Position += character.Length;
			OnUpdateInput?.Invoke(_input);
			OnUpdatePosition?.Invoke(_position);
		}

		public void PressSpecialKey(SpecialKeyType key)
		{
			switch (key)
			{
				case SpecialKeyType.Submit:
					SubmitInput();
					break;
				case SpecialKeyType.Spacebar:
					PressCharacterKey(" ");
					break;
				case SpecialKeyType.Enter:
					PressCharacterKey("\n");
					break;
				case SpecialKeyType.Backspace:
					if (_position == 0) return;
					_input = _input.Remove(_position - 1, 1);
					Position--;
					OnUpdateInput.Invoke(_input);
					break;
				case SpecialKeyType.Delete:
					if (_position >= _input.Length) return;
					_input = _input.Remove(_position, 1);
					OnUpdateInput.Invoke(_input);
					break;
				case SpecialKeyType.DeleteAll:
					_input = string.Empty;
					OnUpdateInput.Invoke(_input);
					break;
				case SpecialKeyType.LeftArrow:
					Position--;
					OnUpdatePosition?.Invoke(_position);
					break;
				case SpecialKeyType.RightArrow:
					Position++;
					OnUpdatePosition?.Invoke(_position);
					break;
				case SpecialKeyType.Shift:
					IsShifted = !_isShifted;
					break;
				case SpecialKeyType.Tab:
					PressCharacterKey("    ");
					break;
			}
		}
	}
}
