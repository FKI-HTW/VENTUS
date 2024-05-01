using UnityEngine;
using VENTUS.DataStructures.Variables;

namespace Game.Feature.UI.Scripts
{
    public class InFrontOfPlayerPlacement : MonoBehaviour
    {
        [SerializeField] private CameraVariable _cameraVariable;
        [SerializeField] private float _distanceFromCamera;
        [SerializeField] private float _heightAddition;
        
        [Header("Invoke Timing")]
        [SerializeField] private bool _invokeOnAwake;
        [SerializeField] private bool _invokeOnStart;
        
        private void Awake()
        {
            if (_invokeOnAwake)
            {
                PlaceCanvas();
            }
        }

        private void Start()
        {
            if (_invokeOnStart)
            {
                PlaceCanvas();
            }
        }

        public void PlaceCanvas()
        {
            var cameraTransform = _cameraVariable.Get().transform;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 xzCameraForward = new Vector3(cameraForward.x, 0, cameraForward.z);
            var cameraPosition = cameraTransform.position;
            Vector3 placementPosition = new Vector3(cameraPosition.x, cameraPosition.y + _heightAddition, cameraPosition.z);
            transform.position = placementPosition + xzCameraForward.normalized * _distanceFromCamera;
        }
    }
}
