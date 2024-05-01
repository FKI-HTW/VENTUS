using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Interaction.TransformTool.UI.View;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    [CreateAssetMenu(fileName = "new RaycastHitNormalRotator", menuName = "VENTUS/TransformTool/Rotator/RaycastHitNormal")]
    public class RaycastHitNormalRotator : BaseRotator
    {
        [SerializeField] private int _registeredRaycastHitCount;
    
        private RaycastHit[] _raycastHits;

        private void OnEnable()
        {
            _raycastHits = new RaycastHit[_registeredRaycastHitCount];
        }
    
        public override void UpdateRotation(Transform transformToRotate, GameObject selectableGo)
        {
            SelectionUtility.CameraToGameObjectCollisionRaycast(selectableGo, _raycastHits, out RaycastHit foundHit);
            transformToRotate.LookAt(foundHit.point - foundHit.normal);
        }
    }
}
