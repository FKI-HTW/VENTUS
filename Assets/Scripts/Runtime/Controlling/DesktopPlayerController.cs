using UnityEngine;

namespace VENTUS.Controlling
{
    public class DesktopPlayerController : MonoBehaviour
    {
        [SerializeField] private float _height = 1.7f;
        [SerializeField] private float _walkingSpeed = 5;
        [SerializeField] private float _rotationSpeed = 200;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                var rotDeltaY = Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
                var rotDeltaX = -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
                transform.rotation *= Quaternion.Euler(rotDeltaX, rotDeltaY, 0);
                transform.eulerAngles = new(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                
                var posDeltaX = Input.GetAxis("Horizontal") * _walkingSpeed * Time.deltaTime;
                var posDeltaZ = Input.GetAxis("Vertical") * _walkingSpeed * Time.deltaTime;
                var position = transform.position;
                position += transform.right * posDeltaX + transform.forward * posDeltaZ;
                position = new(position.x, _height, position.z);
                transform.position = position;
            }
        }

        public void ResetPlayer()
        {
            transform.SetPositionAndRotation(
                new(0, _height, 0), 
                Quaternion.identity
            );
        }
    }
}
