using System.Globalization;
using TMPro;
using UnityEngine;

namespace VENTUS.UI
{
    public class TextUpdate : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _format;

        public void UpdateText(float value)
        {
            _text.text = value.ToString(_format, CultureInfo.InvariantCulture);
        }
    }
}
