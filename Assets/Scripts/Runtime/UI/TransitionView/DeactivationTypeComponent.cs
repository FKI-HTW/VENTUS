using UnityEngine;

namespace VENTUS.UI
{
    public enum DeactivationType { Disable, Destroy, DestroyImmediate }
    public class DeactivationTypeComponent : MonoBehaviour
    {
        public DeactivationType deactivationType;
    }
}
