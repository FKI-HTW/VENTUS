using UnityEngine;
using VENTUS.DataStructures.Variables;

namespace VENTUS.Controlling
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private CameraVariable _camera;

        [SerializeField] private bool _mirror;

        [Tooltip("Looks the transform X-Rotation to a set value.")]
        [SerializeField] private bool _lockXRotation;
        [SerializeField] private float _xRotation;

        private void LateUpdate()
        {
            if (!_camera.TryGet(out var cam)) return;
            
            transform.rotation = BillboardUtility.GetRotation(transform.position, cam.transform.position, _mirror);
            if (_lockXRotation)
                transform.eulerAngles = new(_xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
