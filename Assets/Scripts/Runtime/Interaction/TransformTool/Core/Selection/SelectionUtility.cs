using UnityEngine;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public static class SelectionUtility
    {
        public static void CameraToGameObjectCollisionRaycast(GameObject target, RaycastHit[] allHits, out RaycastHit foundHit)
        {
            Camera mainCamera = Camera.main;
            foundHit = default;
        
            if (mainCamera == null) return;
        
            Vector3 targetPosition = target.transform.position;
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 direction = targetPosition - cameraPosition;
            float distance = Vector3.Distance(cameraPosition, targetPosition);
            var size = Physics.RaycastNonAlloc(cameraPosition, direction, allHits, distance);

            for (int index = 0; index < size; index++)
            {
                if (allHits[index].collider.gameObject == target)
                {
                    foundHit = allHits[index];
                    return;
                }
            }
        }
    }
}
