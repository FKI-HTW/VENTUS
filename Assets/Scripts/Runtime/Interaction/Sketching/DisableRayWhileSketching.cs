using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;

namespace VENTUS.Interaction.Sketching
{
    [RequireComponent(typeof(XRInteractorLineVisual))]
    public class DisableRayWhileSketching : MonoBehaviour, IInteractionActivatable
    {
        [SerializeField] private bool _isLeft;
        [SerializeField] private XRInteractorLineVisual _lineVisual;
        
        private void Awake()
        {
            _lineVisual ??= GetComponent<XRInteractorLineVisual>();
            InteractionTracking.AddElement(InteractionTypes.Sketching, this);
        }

        private void OnDestroy()
        {
            InteractionTracking.RemoveElement(InteractionTypes.Sketching, this);
        }
        
        public void OnEnableInteraction()
        {
            InteractionController.InteractorDown += StartDraw;
            InteractionController.InteractorUp += StopDraw;
        }

        public void OnDisableInteraction()
        {
            InteractionController.InteractorDown -= StartDraw;
            InteractionController.InteractorUp -= StopDraw;
        }

        private void StartDraw(InteractorGroup interactorGroup)
        {
            switch (interactorGroup.EnabledInteractionInstances.First().SelectionFlags)
            {
                case SingleSelectorFlags.Left:
                    if (_isLeft)
                        _lineVisual.enabled = false;
                    break;
                case SingleSelectorFlags.Right:
                    if (!_isLeft)
                        _lineVisual.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StopDraw(InteractorGroup interactorGroup)
        {
            switch (interactorGroup.EnabledInteractionInstances.First().SelectionFlags)
            {
                case SingleSelectorFlags.Left:
                    if (_isLeft)
                        _lineVisual.enabled = true;
                    break;
                case SingleSelectorFlags.Right:
                    if (!_isLeft)
                        _lineVisual.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
