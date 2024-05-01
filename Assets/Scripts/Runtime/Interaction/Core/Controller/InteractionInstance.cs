using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VENTUS.Interaction.Core.Controller
{
    public class InteractionInstance : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private SingleSelectorFlags _selectorFlags;
        [SerializeField] private InputActionProperty _interactionInput;
        [SerializeField] private List<InteractFilter> _selectorConditions;

        public Transform Origin => _origin;
        public SingleSelectorFlags SelectionFlags => _selectorFlags;
        public InteractionController InteractionController { get; private set; }
        public InteractorGroup InteractorGroup { get; private set; }

        private Func<InteractorGroup, bool> _tryInvokeInteractorDown;
        private Action<InteractorGroup> _invokeInteractorUp;
        private bool _interactionSuccessful;
        private bool _initialized;

        public void Initialize(InteractionController interactionController, InteractorGroup interactorGroup, 
            Func<InteractorGroup, bool> invokeInteractorDown, Action<InteractorGroup> invokeInteractorUp)
        {
            InteractionController = interactionController;
            InteractorGroup = interactorGroup;
            _tryInvokeInteractorDown = invokeInteractorDown;
            _invokeInteractorUp = invokeInteractorUp;
            _initialized = true;

            if (isActiveAndEnabled && !InteractionController.IsUnityNull())
            {
                InteractionController.EnableInteractionInstance(this);
            }
        }

        private void OnEnable()
        {
            if (!InteractionController.IsUnityNull())
            {
                InteractionController.EnableInteractionInstance(this);
            }

            if (_interactionInput.action != null)
            {
                _interactionInput.action.started += InteractionStarted;
                _interactionInput.action.canceled += InteractionCanceled;
                _interactionInput.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (!InteractionController.IsUnityNull())
            {
                InteractionController.DisableInteractionInstance(this);
            }

            if (_interactionInput.action != null)
            {
                _interactionInput.action.started -= InteractionStarted;
                _interactionInput.action.canceled -= InteractionCanceled;
                _interactionInput.action.Disable();
            }
        }

        public void Terminate()
        {
            if (isActiveAndEnabled && !InteractionController.IsUnityNull())
            {
                InteractorGroup?.DisableInteractionInstance(this);
            }
            
            _initialized = false;
            InteractionController = null;
            InteractorGroup = null;
        }
    
        private void InteractionStarted(InputAction.CallbackContext context)
        {
            if (!_initialized || _selectorConditions.Any(selectorCondition => !selectorCondition.CanInteract())) return;

            if (_tryInvokeInteractorDown(InteractorGroup))
            {
                _interactionSuccessful = true;
            }
        }

        private void InteractionCanceled(InputAction.CallbackContext context)
        {
            if (!_initialized) return;
            
            if (_interactionSuccessful)
            {
                _invokeInteractorUp(InteractorGroup);
                _interactionSuccessful = false;
            }
        }
    }
}
