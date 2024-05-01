using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    [CreateAssetMenu(fileName = "new SidewardsDistancePositioner", menuName = "VENTUS/TransformTool/Positioner/Sidewards Distance")]
    public class DirectionalDistancePositioner : DistancePositioner
    {
        [Header("Direction")]
        [SerializeField] private float _viewPrefabInstantiationOffset = 0.7f;
        [SerializeField] private Vector3 _viewPrefabInstantiationDirection = Vector3.right;
    
        public override Vector3 GetUpdatedPosition(GameObject selectableGo)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return Vector3.zero;

            Vector3 distancePosition = GetDistancePosition(mainCamera, selectableGo);
            Quaternion cameraToDistancePositionRotation = Quaternion.LookRotation(distancePosition - mainCamera.transform.position);
            
            return distancePosition + ((cameraToDistancePositionRotation * _viewPrefabInstantiationDirection) * _viewPrefabInstantiationOffset);
        }
    }
}
