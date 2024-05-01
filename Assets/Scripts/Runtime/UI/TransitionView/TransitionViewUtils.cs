using System.Collections;
using UnityEngine;

namespace VENTUS.UI.TransitionView
{
    public static class TransitionViewUtils
    {
        public static IEnumerator EnableCoroutine(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                yield return customTransitionView.EnableCoroutine();
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        
        public static void Enable(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                customTransitionView.Enable();
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        
        public static IEnumerator DisableCoroutine(this GameObject gameObject, DeactivationType deactivationType)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                yield return customTransitionView.DisableCoroutine(deactivationType);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        public static void Disable(this GameObject gameObject, DeactivationType deactivationType)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                customTransitionView.Disable(deactivationType);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        public static IEnumerator DisableCoroutine(this GameObject gameObject, DeactivationTypeComponent deactivationType)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                yield return customTransitionView.DisableCoroutine(deactivationType);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        public static void Disable(this GameObject gameObject, DeactivationTypeComponent deactivationType)
        {
            if (gameObject.TryGetComponent(out ICustomTransitionView customTransitionView))
            {
                customTransitionView.Disable(deactivationType);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
