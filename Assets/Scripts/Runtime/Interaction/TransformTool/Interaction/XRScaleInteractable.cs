using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VENTUS.Interaction.TransformTool.Interaction
{
    public class XRScaleInteractable : XRTransformerInteractable
    {
        [SerializeField] private float _smoothSpeed = 15f;

        private float _grabOriginDistance;
        private Vector3 _entryPosition;
        private float _entryDistance;
        private Vector3 _entryScale;

        protected override void OnSelectUpdate()
        {
            float distanceForScale = GetDistanceForScale(_entryPosition, _grabOriginDistance) / _entryDistance;
            Vector3 distanceVector = new Vector3(distanceForScale * _entryScale.x, distanceForScale * _entryScale.y, distanceForScale * _entryScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, distanceVector, _smoothSpeed * Time.deltaTime);
        }

        protected override void InternalOnFirstSelectEntered()
        {
            var thisPosition = transform.position;
            _grabOriginDistance = Vector3.Distance(InteractorGroup.EnabledInteractionInstances.First().Origin.position, thisPosition);
            _entryPosition = thisPosition;
            _entryScale = transform.localScale;
            _entryDistance = GetDistanceForScale(_entryPosition, _grabOriginDistance);
        }

        private float GetDistanceForScale(Vector3 entryPosition, float grabOriginDistance)
        {
            Vector3 interactorForwardVector = InteractorGroup.EnabledInteractionInstances.First().Origin.forward;
            Vector3 directionVector = Quaternion.LookRotation(interactorForwardVector) * Vector3.forward * grabOriginDistance;
            Vector3 targetPosition = InteractorGroup.EnabledInteractionInstances.First().Origin.position + directionVector;
            return Vector3.Distance(entryPosition, targetPosition) * 2;
        }
    }
}
