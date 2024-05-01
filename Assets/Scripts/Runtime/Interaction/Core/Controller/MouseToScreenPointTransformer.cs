using UnityEngine;
using UnityEngine.InputSystem;

namespace VENTUS.Interaction.Core.Controller
{
    public class MouseToScreenPointTransformer : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _originDistanceFromCamera;
        
        void Update()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, _originDistanceFromCamera);
            Ray ray = _camera.ScreenPointToRay(mousePos);
            transform.position = ray.origin;
            transform.forward = ray.direction;
        }
    }
}
