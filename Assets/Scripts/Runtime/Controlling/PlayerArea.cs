using UnityEngine;
using VENTUS.DataStructures.Variables;

namespace VENTUS.Controlling
{
    public class PlayerArea : MonoBehaviour
    {
	    [SerializeField] private CameraVariable _camera;
        [SerializeField] private MeshRenderer _renderer;

		private void LateUpdate()
		{
			if (!_camera.TryGet(out var cam)) return;
			
            var playerPosition = cam.transform.position;
            transform.position = new(playerPosition.x, transform.position.y, playerPosition.z);
            if (_renderer)
				_renderer.material.mainTextureOffset = new(-playerPosition.x, -playerPosition.z);
        }
    }
}
