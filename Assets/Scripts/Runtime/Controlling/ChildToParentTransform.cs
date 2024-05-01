using FishNet.Object;
using UnityEngine;

namespace VENTUS.Controlling
{
    public class ChildToParentTransform : NetworkBehaviour
    {
        public Transform ReferenceTransform { get; set; }
        private Vector3 LocalPosition { get; set; }
        private Quaternion LocalRotation { get; set; }

        private bool _isInitialized;
        private Quaternion _referenceBaseRotation = Quaternion.identity;
        private Vector3 _referenceBaseScale = Vector3.zero;

        private void ResetLocalPosition()
        {
            LocalPosition = transform.position - ReferenceTransform.position;
        }
    
        private void ResetLocalRotation()
        {
            LocalRotation = transform.rotation;
        }

        private void ResetReferenceRotation()
        {
            _referenceBaseRotation = ReferenceTransform.rotation;
        }
    
        private void ResetReferenceScale()
        {
            _referenceBaseScale = ReferenceTransform.lossyScale;
        }

        private Vector3 CalculateNewScale()
        {
            Vector3 lossyScale = Quaternion.Inverse(_referenceBaseRotation) * ReferenceTransform.lossyScale;
            Vector3 baseScale = Quaternion.Inverse(_referenceBaseRotation) * _referenceBaseScale;
            return new Vector3(lossyScale.x / baseScale.x, 
                lossyScale.y / baseScale.y, 
                lossyScale.z / baseScale.z);
        }

        private Quaternion CalculateNewRotation()
        {
            return ReferenceTransform.rotation * Quaternion.Inverse(_referenceBaseRotation);
        }

        private void Update()
        {
            if (!IsSpawned || ReferenceTransform == null) return;

            if (!_isInitialized)
            {
                ResetLocalPosition();
                ResetLocalRotation();
            
                ResetReferenceRotation();
                ResetReferenceScale();
                _isInitialized = true;
            }

            transform.position = ReferenceTransform.position + (CalculateNewRotation() * Vector3.Scale(CalculateNewScale(), LocalPosition));
            transform.rotation = CalculateNewRotation() * LocalRotation;
        }
    }
}
