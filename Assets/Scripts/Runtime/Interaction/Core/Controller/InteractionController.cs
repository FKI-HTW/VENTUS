using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VENTUS.Interaction.Core.Controller
{
    public class InteractionController : MonoBehaviour
    {
        public static event Action<InteractorGroup> InteractorDown;
        public static event Action<InteractorGroup> InteractorUp;

        [SerializeField] private List<InteractorGroup> _interactionGroups;
        [SerializeField] private List<InteractFilter> _globalSelectorConditions;
        
        
        #region lifecycle
        private void Awake()
        {
            foreach (var interactionInstanceContainer in _interactionGroups)
            {
                foreach (var interactionInstance in interactionInstanceContainer.AllInteractionInstances)
                {
                    interactionInstance.Initialize(this, interactionInstanceContainer, TryInvokeInteractorDown, InvokeInteractorUp);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var interactionInstanceContainer in _interactionGroups)
            {
                foreach (var interactionInstance in interactionInstanceContainer.AllInteractionInstances)
                {
                    interactionInstance.Terminate();
                }
            }
        }

        private bool TryInvokeInteractorDown(InteractorGroup interactorInstanceContainer)
        {
            if (_globalSelectorConditions.Any(selectorCondition => !selectorCondition.CanInteract())) return false;
            
            InteractorDown?.Invoke(interactorInstanceContainer);
            return true;
        }

        private void InvokeInteractorUp(InteractorGroup interactorInstanceContainer)
        {
            InteractorUp?.Invoke(interactorInstanceContainer);
        }

        public void EnableInteractionInstance(InteractionInstance interactionInstance)
        {
            foreach (InteractorGroup interactionGroup in _interactionGroups)
            {
                if (interactionGroup.AllInteractionInstances.Contains(interactionInstance))
                {
                    interactionGroup.EnableInteractionInstance(interactionInstance);
                }
            }
        }

        public void DisableInteractionInstance(InteractionInstance interactionInstance)
        {
            foreach (InteractorGroup interactionGroup in _interactionGroups)
            {
                if (interactionGroup.AllInteractionInstances.Contains(interactionInstance))
                {
                    interactionGroup.DisableInteractionInstance(interactionInstance);
                }
            }
        }
        
        #endregion
    }
    
    [Serializable]
    public class InteractorGroup
    {
        [SerializeField] private List<InteractionInstance> _allInteractionInstances;

        public List<InteractionInstance> AllInteractionInstances => _allInteractionInstances;

        public List<InteractionInstance> EnabledInteractionInstances { get; private set; } = new();

        public void EnableInteractionInstance(InteractionInstance interactionInstance)
        {
            if (!_allInteractionInstances.Contains(interactionInstance))
            {
                Debug.LogError("Cant enable an interaction instance, that is not part of the same group");
            }

            if (EnabledInteractionInstances.Contains(interactionInstance))
            {
                Debug.LogWarning("Cant enable an interaction instance, that is activated inside the group");
                return;
            }
            
            EnabledInteractionInstances.Add(interactionInstance);
        }

        public void DisableInteractionInstance(InteractionInstance interactionInstance)
        {
            if (!_allInteractionInstances.Contains(interactionInstance))
            {
                Debug.LogError("Cant deactivate an interaction instance, that is not part of the same group");
            }
            
            if (!EnabledInteractionInstances.Contains(interactionInstance))
            {
                Debug.LogWarning("Cant deactivate an interaction instance, that is not activated inside the group");
                return;
            }
            
            EnabledInteractionInstances.Remove(interactionInstance);
        }
    }
}
