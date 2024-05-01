using UnityEngine;

namespace VENTUS.Controlling
{
    public static class BillboardUtility
    {
        public static void UpdateRotation(Transform ownerTransform, Transform targetTransform, bool mirror)
        {
            if (mirror)
            {
                ownerTransform.rotation = Quaternion.LookRotation(ownerTransform.position - targetTransform.position);
            }
            else
            {
                ownerTransform.LookAt(targetTransform);
            }
        }
    
        public static Quaternion GetRotation(Vector3 ownerPosition, Vector3 targetPosition, bool mirror)
        {
            if (ownerPosition - targetPosition == Vector3.zero) return Quaternion.identity;
        
            if (mirror)
            {
                return Quaternion.LookRotation(ownerPosition - targetPosition);
            }
            else
            {
                return Quaternion.LookRotation(targetPosition - ownerPosition);
            }
        }
    }
}
