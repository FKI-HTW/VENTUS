using Unity.XR.CoreUtils;
using UnityEngine;

namespace VENTUS.PlaformPackageExtension
{
    public class ParentLayerInheritor : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetLayerRecursively(transform.parent.gameObject.layer);
        }
    }
}
