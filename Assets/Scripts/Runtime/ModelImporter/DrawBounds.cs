using UnityEngine;

namespace VENTUS.ModelImporter
{
    public class DrawBounds : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private bool _drawBoundingBox;
        
        private void OnDrawGizmos()
        {
            if (!_drawBoundingBox) return;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.red;
            var bounds = _meshRenderer.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
