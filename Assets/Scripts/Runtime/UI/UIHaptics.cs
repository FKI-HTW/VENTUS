using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace VENTUS.UI
{
    public class UIHaptics : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private float _amplitude = 0.5f;
        [SerializeField] private float _duration = 0.05f;
        
        [Header("Optional Require Interactable")]
        [SerializeField] private Selectable _selectable;
        
        private BaseInputModule InputModule => EventSystem.current?.currentInputModule;
 
 
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable)
                return;
                
            SendHaptics(eventData);
        }
 
        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable)
                return;
            
            SendHaptics(eventData);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable)
                return;
            
            SendHaptics(eventData);
        }

        private void SendHaptics(PointerEventData eventData)
        {
            if (InputModule is not XRUIInputModule xrInputModule) return;
            
            var interactor = xrInputModule.GetInteractor(eventData.pointerId) as XRBaseControllerInteractor;
            if (!interactor) return;
     
            interactor.xrController.SendHapticImpulse(_amplitude, _duration);

        }
    }
}
