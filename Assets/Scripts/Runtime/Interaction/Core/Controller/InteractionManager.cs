using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VENTUS.Interaction.Core.Controller
{
    public enum SingleSelectorFlags { Left, Right }
    
    public class InteractionManager : MonoBehaviour
    {
        public static event Action<InputAction.CallbackContext, Interactor> InteractorDown;
        public static event Action<InputAction.CallbackContext, Interactor> InteractorUp;
        
        #region fields
        [SerializeField] private List<InteractFilter> _globalSelectorConditions;
        [SerializeField] private List<InteractorData> _selectorMapList;

        private readonly List<Interactor> _selectorContainers = new ();

        private readonly List<GameObject> _controllerSelecting = new List<GameObject>();
        
        #endregion

        #region lifecycle
        private void Awake()
        {
            foreach (var selectableMap in _selectorMapList)
            {
                Interactor interactor = new Interactor(this, selectableMap);
                interactor.Initialize();
                _selectorContainers.Add(interactor);
            }
        }

        private void OnDestroy()
        {
            for (var index = _selectorContainers.Count - 1; index >= 0; index--)
            {
                var selectorContainer = _selectorContainers[index];
                selectorContainer.Terminate();
            }
        }
        #endregion

        [Serializable]
        public class InteractorData
        {
            public Transform _origin;
            public SingleSelectorFlags _singleSelectorFlags;
            public InputActionProperty _interactionInput;
            public List<InteractFilter> _selectorConditions;
        }
    
        public class Interactor
        {
            public InteractionManager InteractionManager { get; }
            public InteractorData InteractorData { get; }
        
            private bool _interactionSuccessful;

            public Interactor(InteractionManager interactionManager, InteractorData interactorData)
            {
                InteractionManager = interactionManager;
                InteractorData = interactorData;
            }

            public void Initialize()
            {
                if (InteractorData._interactionInput.action != null)
                {
                    InteractorData._interactionInput.action.started += InteractionStarted;
                    InteractorData._interactionInput.action.canceled += InteractionCanceled;
                    InteractorData._interactionInput.action.Enable();
                }
            }

            public void Terminate()
            {
                if (InteractorData._interactionInput.action != null)
                {
                    InteractorData._interactionInput.action.started -= InteractionStarted;
                    InteractorData._interactionInput.action.canceled -= InteractionCanceled;
                    InteractorData._interactionInput.action.Disable();
                }
            }
        
            private void InteractionStarted(InputAction.CallbackContext context)
            {
                if (InteractionManager._controllerSelecting.Contains(InteractorData._origin.gameObject)) return;
                InteractionManager._controllerSelecting.Add(InteractorData._origin.gameObject);
                
                if (InteractionManager._globalSelectorConditions.Any(selectorCondition => !selectorCondition.CanInteract())) return;
                if (InteractorData._selectorConditions.Any(selectorCondition => !selectorCondition.CanInteract())) return;
                
                InteractorDown?.Invoke(context, this);
                _interactionSuccessful = true;
            }

            private void InteractionCanceled(InputAction.CallbackContext context)
            {
                if (_interactionSuccessful)
                {
                    _interactionSuccessful = false;
                    InteractorUp?.Invoke(context, this);
                }
                
                if (InteractionManager._controllerSelecting.Contains(InteractorData._origin.gameObject))
                {
                    InteractionManager._controllerSelecting.Remove(InteractorData._origin.gameObject);
                }
            }
        }
    }
}
