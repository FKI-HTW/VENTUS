using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VENTUS.Interaction.Core.Controller
{
    public class OverUIFilterInteractor : InteractFilter
    {
        [Header("Disable Over Ui")]
        [SerializeField] private XRRayInteractor _rayInteractor;
        [SerializeField] private bool _disableOverUI;
    
        private void Awake()
        {
            if (_rayInteractor == null)
                _rayInteractor = GetComponent<XRRayInteractor>();
        }
    
        public override bool CanInteract()
        {
            return !_disableOverUI || !_rayInteractor.IsOverUIGameObject();
        }
    }
}
