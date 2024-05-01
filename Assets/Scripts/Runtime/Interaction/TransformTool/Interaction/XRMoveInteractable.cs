using System.Linq;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core;

namespace VENTUS.Interaction.TransformTool.Interaction
{
    public class XRMoveInteractable : XRTransformerInteractable
    {
        [SerializeField] private float _smoothSpeed = 15f;
    
        private float _grabOriginDistance;
        private Vector3 _originOffset;

        protected override void OnSelectUpdate()
        {
            Transform interactorTransform = InteractorGroup.EnabledInteractionInstances.First().Origin;
            Vector3 directionVector = GetDirectionVector(interactorTransform.forward, _grabOriginDistance);
            Vector3 targetPosition = interactorTransform.position + directionVector + _originOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
        }

        protected override void InternalOnFirstSelectEntered()
        {
            Transform interactorTransform = InteractorGroup.EnabledInteractionInstances.First().Origin;
            Vector3 interactorPosition = interactorTransform.position;
            Vector3 thisPosition = transform.position;
            _grabOriginDistance = Vector3.Distance(interactorPosition, thisPosition);
            _originOffset = thisPosition - (interactorPosition + GetDirectionVector(interactorTransform.forward, _grabOriginDistance));
        }

        private Vector3 GetDirectionVector(Vector3 forward, float grabOriginDistance)
        {
            return Quaternion.LookRotation(forward) * Vector3.forward * grabOriginDistance;
        }
    }
}
