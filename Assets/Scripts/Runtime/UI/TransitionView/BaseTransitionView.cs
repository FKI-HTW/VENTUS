using System;
using System.Collections;
using UnityEngine;

namespace VENTUS.UI
{
    public enum TransitionStartType { Automatic, Manuel, Inactive }
    
    [DisallowMultipleComponent]
    public abstract class BaseTransitionView : MonoBehaviour, ICustomTransitionView
    {
        [SerializeField] private TransitionStartType _transitionStartType;

        private bool _hasAlreadyBeenEnabled;

        /// <summary>
        /// Implements support for custom enabling
        /// </summary>
        private void Start()
        {
            // If Enable(); was already called, it was already enabled in a custom way
            if (_hasAlreadyBeenEnabled) return;
            
            OnSetup();
            
            switch (_transitionStartType)
            {
                case TransitionStartType.Automatic:
                    Enable();
                    break;
                case TransitionStartType.Manuel:
                    break;
                case TransitionStartType.Inactive:
                    // If this is inside an awake, the Awake(); method of other components on this gameObject or it's child might not be invoked. This is due to OnStartInternals gameObject.SetActive(false);
                    // If we put the gameObject.SetActive(false); inside OnSetupDisabled(); the Coroutine won't be started, cause inactive gameObject.
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Enable()
        {
            PrepareEnable();
            StartCoroutine(OnSetActiveTransition());
        }
        
        public IEnumerator EnableCoroutine()
        {
            PrepareEnable();
            yield return OnSetActiveTransition();
        }

        private void PrepareEnable()
        {
            if (!_hasAlreadyBeenEnabled)
            {
                _hasAlreadyBeenEnabled = true;
                OnSetup();
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        
        public void Disable(DeactivationType deactivationType)
        {
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"Coroutine couldn't be started because the game object '{name}' is inactive!");
                return;
            }
            
            StartCoroutine(DisableCoroutine(deactivationType));
        }

        public IEnumerator DisableCoroutine(DeactivationType deactivationType)
        {
            yield return OnSetInactiveTransition();
            PerformDeactivation(deactivationType);
        }
        
        public void Disable(DeactivationTypeComponent deactivationComponent)
        {
            Disable(deactivationComponent.deactivationType);
        }
        
        public IEnumerator DisableCoroutine(DeactivationTypeComponent deactivationComponent)
        {
            yield return DisableCoroutine(deactivationComponent.deactivationType);
        }

        protected abstract void OnSetup();

        protected abstract IEnumerator OnSetActiveTransition();

        protected abstract IEnumerator OnSetInactiveTransition();

        protected void PerformDeactivation(DeactivationType deactivationType)
        {
            switch (deactivationType)
            {
                case DeactivationType.Disable:
                    gameObject.SetActive(false);
                    break;
                case DeactivationType.Destroy:
                    Destroy(gameObject);
                    break;
                case DeactivationType.DestroyImmediate:
                    DestroyImmediate(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deactivationType), deactivationType, null);
            }
        }
    }
}
