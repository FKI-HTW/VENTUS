using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VENTUS.Interaction.Core.Interaction
{
    public class InteractionChangeEvent : MonoBehaviour, IInteractionActivatable
    {
        [SerializeField] private List<InteractionTypes> _interactionTypes;
        [SerializeField] private UnityEvent onInitializeInteractionEvent;
        [SerializeField] private UnityEvent onEnableInteractionEvent;
        [SerializeField] private UnityEvent onDisableInteractionEvent;
    
        private void Awake()
        {
            foreach (var interactionTypes in _interactionTypes)
            {
                InteractionTracking.AddElement(interactionTypes, this);
            }
        }

        private void OnDestroy()
        {
            foreach (var interactionTypes in _interactionTypes)
            {
                InteractionTracking.RemoveElement(interactionTypes, this);
            }
        }

        private void Start()
        {
            onInitializeInteractionEvent?.Invoke();
        }

        public void OnEnableInteraction()
        {
            onEnableInteractionEvent?.Invoke();
        }

        public void OnDisableInteraction()
        {
            onDisableInteractionEvent?.Invoke();
        }
    }
}
