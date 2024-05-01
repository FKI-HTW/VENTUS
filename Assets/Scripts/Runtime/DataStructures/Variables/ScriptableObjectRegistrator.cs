using System;
using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
    public class ScriptableObjectRegistrator : MonoBehaviour
    {
        [SerializeField] private GameObject _targetGameObject;
        [SerializeField] private ScriptableObject _scriptableObject;
    
        [Header("Register")]
        [SerializeField] private bool _registerOnAwake;
        [SerializeField] private bool _registerOnStart;
        [SerializeField] private bool _registerOnEnable;
    
        [Header("Unregister")]
        [SerializeField] private bool _unregisterOnDestroy;
        [SerializeField] private bool _unregisterOnDisable;

        private void Awake()
        {
            if (!_registerOnAwake) return;

            PerformScriptableObjectRegisterAction(register => register.Register(_targetGameObject != null ? _targetGameObject : gameObject));
        }

        private void OnEnable()
        {
            if (!_registerOnEnable) return;
        
            PerformScriptableObjectRegisterAction(register => register.Register(_targetGameObject != null ? _targetGameObject : gameObject));
        }

        private void Start()
        {
            if (!_registerOnStart) return;
        
            PerformScriptableObjectRegisterAction(register => register.Register(_targetGameObject != null ? _targetGameObject : gameObject));
        }
    
        private void OnDisable()
        {
            if (!_unregisterOnDestroy) return;
        
            PerformScriptableObjectRegisterAction(register => register.Unregister());
        }

        private void OnDestroy()
        {
            if (!_unregisterOnDisable) return;
        
            PerformScriptableObjectRegisterAction(register => register.Unregister());
        }
    
        private void PerformScriptableObjectRegisterAction(Action<IScriptableObjectRegister> action)
        {
            if (_scriptableObject is IScriptableObjectRegister focusRegister)
            {
                action.Invoke(focusRegister);
            }
            else
            {
                Debug.LogWarning($"You Need to implement {typeof(IScriptableObjectRegister)} on {_scriptableObject}");
            }
        }
    }

    public interface IScriptableObjectRegister
    {
        void Register(GameObject relatedGameObject);
        void Unregister();
    }
}