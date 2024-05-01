using UnityEngine;

namespace VENTUS.Interaction.Core.Controller
{
    public abstract class InteractFilter : MonoBehaviour
    {
        public abstract bool CanInteract();
    }
}
