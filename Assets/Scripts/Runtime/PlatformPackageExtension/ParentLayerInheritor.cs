using Unity.XR.CoreUtils;
using UnityEngine;

namespace VENTUS.PlatformPackageExtension
{
    public class ParentLayerInheritor : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetLayerRecursively(transform.parent.gameObject.layer);
        }
    }
}
