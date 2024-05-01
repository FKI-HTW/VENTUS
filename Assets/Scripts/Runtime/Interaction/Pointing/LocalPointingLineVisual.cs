using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;

namespace VENTUS.Interaction.Pointing
{
    public class LocalPointingLineVisual : MonoBehaviour, IInteractionActivatable
    {
        [SerializeField] private bool _isLeft = true;
        
        private void Awake()
        {
            InteractionTracking.AddElement(InteractionTypes.Pointing, this);
        }

        private void OnDestroy()
        {
            InteractionTracking.RemoveElement(InteractionTypes.Pointing, this);
        }

        public void OnEnableInteraction()
        {
            InteractionController.InteractorDown += DisableLine;
            InteractionController.InteractorUp += EnableLine;
        }

        public void OnDisableInteraction()
        {
            InteractionController.InteractorDown -= DisableLine;
            InteractionController.InteractorUp -= EnableLine;
            
            gameObject.SetActive(true);
        }
        
        private void EnableLine(InteractorGroup interactorGroup)
        {
            if (_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Left)
                return;

            if (!_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Right)
                return;
            
            gameObject.SetActive(true);
        }

        private void DisableLine(InteractorGroup interactorGroup)
        {
            if (_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Left)
                return;

            if (!_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Right)
                return;
            
            gameObject.SetActive(false);
        }
    }
}
