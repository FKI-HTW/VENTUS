using System.Linq;
using UnityEngine;
using VENTUS.Interaction.Core.Controller;

namespace VENTUS.Interaction.TransformTool.Core
{
    public class GrabTransformCalculator
    {
        private readonly InteractorGroup _interactorGroup;
        private readonly Transform _selectedTransform;

        private Quaternion _previousControllerRotation;
        private Vector3 _previousControllerPosition;
        
        private Quaternion _previousRotation;
        private Quaternion _targetRotation;
        
        private Vector3 _targetPosition;
        private Vector3 _previousPosition;
    
        public GrabTransformCalculator(InteractorGroup interactorGroup, Transform selectedTransform, float maxDistance, bool snapToHand)
        {
            _interactorGroup = interactorGroup;
            _selectedTransform = selectedTransform;
        
            _previousControllerPosition = interactorGroup.EnabledInteractionInstances.First().Origin.position;
            _previousControllerRotation = interactorGroup.EnabledInteractionInstances.First().Origin.rotation;
        
            _targetRotation = Quaternion.identity;
            _previousRotation = Quaternion.identity;
        
            if (Physics.Raycast(interactorGroup.EnabledInteractionInstances.First().Origin.position, 
                    interactorGroup.EnabledInteractionInstances.First().Origin.forward, 
                    out RaycastHit raycastHit, maxDistance, 1 << selectedTransform.gameObject.layer) && snapToHand)
            {
                Vector3 hitPointDirection = selectedTransform.position - raycastHit.point;
                selectedTransform.position = interactorGroup.EnabledInteractionInstances.First().Origin.position + hitPointDirection;
            }

            var selectedPosition = selectedTransform.position;
            _previousPosition = selectedPosition;
            _targetPosition = selectedPosition;
        }

        public void UpdateTransformation(float smoothSpeed)
        {
            /*
             * Just using:
             * Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
             * or:
             * Quaternion.Lerp(transform.rotation, targetRotation, _smoothSpeed * Time.deltaTime);
             * doesn't work here. It would just reduce the movement and not align to the target.
             * In order to prevent that behaviour, we need to buffer the result of the smoothed value for the next frame.
             */
            
            Transform selectorTransform = _interactorGroup.EnabledInteractionInstances.First().Origin;
            Quaternion selectorRotation = selectorTransform.rotation;
            Vector3 selectorPosition = selectorTransform.position;
            
            //Prepare new world space position which is used as the origin for the rotate around
            Vector3 newTargetOrigin = _targetPosition + (selectorPosition - _previousControllerPosition);
            _previousControllerPosition = selectorPosition;
            
            //Prepare the divergence between previous and current rotation of the controller
            Quaternion selectorRotationDelta = selectorRotation * Quaternion.Inverse(_previousControllerRotation);
            _previousControllerRotation = selectorRotation;
            
            //Positioning by the rotate around formula: originPoint = rotation * (originPoint - pivotPoint) + pivotPoint
            //The used coordinate-space for interpolating to the new position is the world space
            _targetPosition = selectorRotationDelta * (newTargetOrigin - selectorPosition) + selectorPosition;
            Vector3 smoothedPosition = Vector3.Lerp(_previousPosition, _targetPosition, smoothSpeed * Time.deltaTime);
            _selectedTransform.position = smoothedPosition;
            _previousPosition = smoothedPosition;

            //Rotating by the rotate around formula: originRotation = addedRotation * originRotation
            //The used coordinate-space for interpolating to the new rotation is the relative local space
            _targetRotation = selectorRotationDelta * _targetRotation;
            Quaternion smoothedRotation = Quaternion.Lerp(_previousRotation, _targetRotation, smoothSpeed * Time.deltaTime);
            _selectedTransform.rotation = smoothedRotation * Quaternion.Inverse(_previousRotation) * _selectedTransform.rotation;
            _previousRotation = smoothedRotation;
        }
    }
}