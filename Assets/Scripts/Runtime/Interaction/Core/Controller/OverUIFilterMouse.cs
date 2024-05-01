using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace VENTUS.Interaction.Core.Controller
{
    public class OverUIFilterMouse : InteractFilter
    {
        [SerializeField] private bool _disableOverUI;
        
        public override bool CanInteract()
        {
            return !_disableOverUI || !IsOverUIGameObject();
        }

        private bool IsOverUIGameObject()
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mouseScreenPos;
            
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
 
            return raycastResults.Count > 0;
        }
    }
}
