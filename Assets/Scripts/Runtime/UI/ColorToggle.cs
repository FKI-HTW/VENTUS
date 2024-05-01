using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VENTUS.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ColorToggle : MonoBehaviour
    {
        [SerializeField] private Image _toggleColorImage;

        public UnityEvent<Color> OnToggled; 

        public void ActivateToggle(bool isToggled)
        {
            if (isToggled)
                OnToggled?.Invoke(_toggleColorImage.color);
        }
    }
}
