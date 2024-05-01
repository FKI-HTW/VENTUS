using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.UI
{
    /// <summary>
    /// Needed Sprite Import Settings: You need to set the Read/Write to true and MeshType to FullRect in order for it to work!
    /// </summary>
    public class ImageRaycastHitAlphaMinimumThreshold : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _minAlpha = 0.3f;

        private void Awake()
        {
            _image.alphaHitTestMinimumThreshold = _minAlpha;
        }
    }
}
