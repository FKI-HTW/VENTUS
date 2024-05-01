using UnityEngine;
using Unity.Mathematics;
using VENTUS.DataStructures.Variables;

namespace VENTUS.Controlling
{
    public class AlphaDistance : MonoBehaviour
    {
        [SerializeField] private CameraVariable _camera;
        [SerializeField] private Material _transparencyMaterial;
        [SerializeField] private float _minDistance = 20;
        [SerializeField] private float _maxDistance = 300;

        private void LateUpdate()
        {
            if (!_camera.TryGet(out var cam)) return;
            var pos = cam.transform.position;
            
            var distance = Vector3.Distance(new(pos.x, 0, pos.x), Vector3.zero);
            var alpha = math.remap(_minDistance, _maxDistance, 0, 0.8f, distance);
            alpha = math.clamp(alpha, 0, 1);
            _transparencyMaterial.SetFloat("_TransparencyValue", alpha);
        }
    }
}
