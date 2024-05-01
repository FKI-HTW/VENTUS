using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.TransformTool.Core;

namespace VENTUS.Interaction.TransformTool.Interaction
{
    public class XRGrabInteractable : XRTransformerInteractable
    {
        [SerializeField] private float _smoothSpeed = 15f;
        [SerializeField] private float _maxDistance = 10f;
        [SerializeField] private bool _snapToHand = true;

        private GrabTransformCalculator _grabTransformCalculator;

        protected override void OnSelectUpdate()
        {
            _grabTransformCalculator.UpdateTransformation(_smoothSpeed);
        }

        protected override void InternalOnFirstSelectEntered()
        {
            _grabTransformCalculator = new GrabTransformCalculator(InteractorGroup, transform, _maxDistance, _snapToHand);
        }
    }
}
