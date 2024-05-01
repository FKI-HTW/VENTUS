using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.Interaction.TransformTool.UI.View.PreparedText
{
    public class PreparedTextViewElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;

        public string Text => _text.text;

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetText(string message)
        {
            _text.text = message;
        }

        public void RegisterOnClickAction(Action onClick)
        {
            _button.onClick.AddListener(onClick.Invoke);
        }
    }
}
