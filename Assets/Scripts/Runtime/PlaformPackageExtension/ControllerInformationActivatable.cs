using System.Collections.Generic;
using CENTIS.XRPlatformManagement.Controller.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace VENTUS.PlaformPackageExtension
{
    public class ControllerInformationActivatable : MonoBehaviour
    {
        #region Fields
    
        [SerializeField] private ExtendedControllerModelSpawner _extendedControllerModelSpawner;
        [SerializeField] private bool _deactivateAllInstances;
        [SerializeField] private float _noInputActivationTime;
        [SerializeField] private List<InputActionProperty> _interactionInput;
        [SerializeField] private UnityEvent _onActivate;
        [SerializeField] private UnityEvent _onDeactivate;

        private static HashSet<ControllerInformationActivatable> _instances;
        
        private float _noInputActivationTimeDelta;
        private bool _blockUpdateTime;
        private bool _currentlyActive;
    
        #endregion

        #region Unity Lifecycle
    
        private void Awake()
        {
            _instances ??= new HashSet<ControllerInformationActivatable>();
            _instances.Add(this);
            
            BlockUpdateTime();
            _extendedControllerModelSpawner.RegisterAction(ControllerModelSpawner.EventType.OnFinishTrackingAcquired, _ => UnblockUpdateTime());
            _extendedControllerModelSpawner.RegisterAction(ControllerModelSpawner.EventType.OnFinishTrackingLost, _ => BlockUpdateTime());

            foreach (var inputActionProperty in _interactionInput)
            {
                inputActionProperty.action.started += OnEnableInteraction;
                inputActionProperty.action.canceled += OnDisableInteraction;
            }
            
        }

        private void OnDestroy()
        {
            _instances.Remove(this);
            
            _extendedControllerModelSpawner.UnregisterAction(ControllerModelSpawner.EventType.OnFinishTrackingAcquired, _ => UnblockUpdateTime());
            _extendedControllerModelSpawner.UnregisterAction(ControllerModelSpawner.EventType.OnFinishTrackingLost, _ => BlockUpdateTime());

            foreach (var inputActionProperty in _interactionInput)
            {
                inputActionProperty.action.started -= OnEnableInteraction;
                inputActionProperty.action.canceled -= OnDisableInteraction;
            }
        }

        private void Update()
        {
            if (_blockUpdateTime || _currentlyActive) return;
        
            _noInputActivationTimeDelta += Time.deltaTime;
            if (_noInputActivationTimeDelta >= _noInputActivationTime)
            {
                _onActivate?.Invoke();
                _currentlyActive = true;
            }
        }

        #endregion

        #region Private Methods
        
        private void BlockUpdateTime()
        {
            _blockUpdateTime = true;
            _noInputActivationTimeDelta = 0f;
        }

        private void UnblockUpdateTime()
        {
            _blockUpdateTime = false;
            _noInputActivationTimeDelta = 0f;
        }

        private void OnEnableInteraction(InputAction.CallbackContext context)
        {
            foreach (var instance in _instances)
            {
                if (instance != this && !_deactivateAllInstances) continue;
                
                instance.BlockUpdateTime();

                if (instance._currentlyActive)
                {
                    instance._onDeactivate?.Invoke();
                    instance._currentlyActive = false;
                }
            }
        }

        private void OnDisableInteraction(InputAction.CallbackContext context)
        {
            foreach (var instance in _instances)
            {
                if (instance != this && !_deactivateAllInstances) continue;
                
                instance.UnblockUpdateTime();;
            }
        }

        #endregion
    }
}
