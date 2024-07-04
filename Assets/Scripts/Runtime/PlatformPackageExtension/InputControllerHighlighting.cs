using CENTIS.XRPlatformManagement.Controller.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VENTUS.PlatformPackageExtension
{
    public class InputControllerHighlighting : ControllerMaterialHighlighting
    {
        #region Fields

        [SerializeField] private float _noInputActivationTime;
        [SerializeField] private InputActionProperty _interactionInput;

        private float _noInputActivationTimeDelta;
        private bool _blockUpdateTime = true;
    
        #endregion
    
        #region Unity Lifecycle
    
        protected override void Awake()
        {
            base.Awake();
            _interactionInput.action.started += OnEnableInteraction;
            _interactionInput.action.canceled += OnDisableInteraction;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _interactionInput.action.started -= OnEnableInteraction;
            _interactionInput.action.canceled -= OnDisableInteraction;
        }

        private void Update()
        {
            if (_blockUpdateTime || IsCurrentlyActive) return;
            
            _noInputActivationTimeDelta += Time.deltaTime;
            if (_noInputActivationTimeDelta >= _noInputActivationTime)
            {
                Activate();
            }
        }

        #endregion

        #region Private Methods
    
        private void OnEnableInteraction(InputAction.CallbackContext context)
        {
            _blockUpdateTime = true;
            _noInputActivationTimeDelta = 0f;

            if (IsCurrentlyActive)
            {
                Deactivate();
            }
        }

        private void OnDisableInteraction(InputAction.CallbackContext context)
        {
            _blockUpdateTime = false;
            _noInputActivationTimeDelta = 0f;
        }
    
        #endregion
    }
}
