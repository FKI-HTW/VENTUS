using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;
using UnityObject = UnityEngine.Object;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public class Selector : MonoBehaviour, IInteractionActivatable
    {
        [SerializeField] private float distance = 10f;
        [SerializeField] private bool usesInfiniteDistance;
        [SerializeField] private List<InteractionTypes> _interactionTypes;
        [SerializeField] private LayerMask _selectionLayerMask;

        private List<IUnselectable> _unselectables = new();
        
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

        public void OnEnableInteraction()
        {
            InteractionController.InteractorDown += Select;
            InteractionController.InteractorUp += Unselect;
        }

        public void OnDisableInteraction()
        {
            InteractionController.InteractorDown -= Select;
            InteractionController.InteractorUp -= Unselect;
        }

        private void Select(InteractorGroup interactorGroup)
        {
            if (!Physics.Raycast(interactorGroup.EnabledInteractionInstances.First().Origin.position, interactorGroup.EnabledInteractionInstances.First().Origin.forward, 
                    out RaycastHit raycastHit, usesInfiniteDistance ? float.PositiveInfinity : distance))
                return;

            if (!ContainsLayer(_selectionLayerMask, raycastHit.collider.gameObject.layer))
                return;
            
            ISelectable[] selectables = raycastHit.collider.gameObject.GetComponents<ISelectable>();
            if (selectables.Length != 0)
            {
                foreach (var selectable in selectables)
                {
                    if (selectable.CanSelect)
                    {
                        if (selectable is IUnselectable unselectable)
                        {
                            _unselectables.Add(unselectable);
                        }
                        
                        selectable.Select(interactorGroup);
                    }
                }
            }
        }
        
        private bool ContainsLayer(LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        private void Unselect(InteractorGroup interactorGroup)
        {
            for (var index = _unselectables.Count - 1; index >= 0; index--)
            {
                var unselectable = _unselectables[index];

                if (unselectable is not UnityObject || (unselectable is UnityObject unityObject && !unityObject.IsDestroyed()))
                {
                    unselectable.Unselect();
                }

                _unselectables.Remove(unselectable);
            }
        }
    }
}
