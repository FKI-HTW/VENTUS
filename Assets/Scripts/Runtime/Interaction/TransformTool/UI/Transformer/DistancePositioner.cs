using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    [CreateAssetMenu(fileName = "new DistancePositioner", menuName = "VENTUS/TransformTool/Positioner/Distance")]
    public class DistancePositioner : BasePositioner
    {
        [SerializeField] private float _minDistance;
        [SerializeField, Range(0f, 1f)] private float _percentPlacedTowardsTarget;
    
        public override Vector3 GetUpdatedPosition(GameObject selectableGo)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return Vector3.zero;

            return GetDistancePosition(mainCamera, selectableGo);
        }
        
        protected Vector3 GetDistancePosition(Camera mainCamera, GameObject selectableGo)
        {
            Vector3 mainCameraPosition = mainCamera.transform.position;
            Vector3 selectablePosition = selectableGo.transform.position;

            var percentPosition = Vector3.Lerp(mainCameraPosition, selectablePosition, _percentPlacedTowardsTarget);
            var distance = Vector3.Distance(percentPosition, mainCameraPosition);

            if (distance > _minDistance)
            {
                return percentPosition;
            }

            return mainCameraPosition + (selectablePosition - mainCameraPosition).normalized * _minDistance;
        }
    }
}
