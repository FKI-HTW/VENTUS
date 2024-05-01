using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    [CreateAssetMenu(fileName = "new RaycastHitPointPositioner", menuName = "VENTUS/TransformTool/Positioner/RaycastHitPoint")]
    public class RaycastHitPointPositioner : BasePositioner
    {
        [SerializeField] private int _registeredRaycastHitCount;
        [SerializeField] private float _wallDistance;

        private RaycastHit[] _raycastHits;

        private void OnEnable()
        {
            _raycastHits = new RaycastHit[_registeredRaycastHitCount];
        }

        public override Vector3 GetUpdatedPosition(GameObject selectableGo)
        {
            SelectionUtility.CameraToGameObjectCollisionRaycast(selectableGo, _raycastHits, out RaycastHit foundHit);
            return foundHit.point + foundHit.normal * _wallDistance;;
        }
    }
}
