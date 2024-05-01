using System.Collections;

namespace VENTUS.UI
{
    public interface ICustomTransitionView
    {
        public void Enable();
        public IEnumerator EnableCoroutine();
        public void Disable(DeactivationType deactivationType);
        public IEnumerator DisableCoroutine(DeactivationType deactivationType);
        public void Disable(DeactivationTypeComponent deactivationType);
        public IEnumerator DisableCoroutine(DeactivationTypeComponent deactivationType);
    }
}
