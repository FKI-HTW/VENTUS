using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.Interaction
{
    public abstract class XRTransformerInteractable : MonoBehaviour, ISelectable, IUnselectable
    {
        #region Fields

        public InteractorGroup InteractorGroup { get; set; }

        private bool _isSelected;
        private Vector3 _entryPosition;
        private Quaternion _entryRotation;
        private Vector3 _entryScale;
        
        #endregion
    
        #region Unity Lifecycle

        protected virtual void Update()
        {
            if (!_isSelected) 
                return;
        
            OnSelectUpdate();
        }
        
        #endregion
        
        #region Private Methods
        
        public bool CanSelect => true;
        public void Select(InteractorGroup interactorGroup)
        {
            _isSelected = true;
            InteractorGroup = interactorGroup;
            
            OnGrab();
            InternalOnFirstSelectEntered();
        }

        public void Unselect()
        {
            InternalOnLastSelectExited();
            OnDrop();
            
            _isSelected = false;
        }
        
        private void OnGrab()
        {
            Transform thisTransform = transform;
            _entryPosition = thisTransform.position;
            _entryRotation = thisTransform.rotation;
            _entryScale = thisTransform.localScale;
        }
    
        private void OnDrop()
        {
            Transform thisTransform = transform;
            TransformCommandBroadcastInstaller.BroadcastTransformCommandToServer(gameObject, _entryPosition, thisTransform.position,
                _entryRotation, thisTransform.rotation, _entryScale, thisTransform.localScale);
        }
        
        #endregion
    
        #region Virtual Methods
        
        protected virtual void OnSelectUpdate() { }
    
        protected virtual void InternalOnFirstSelectEntered() { }

        protected virtual void InternalOnLastSelectExited() { }
        
        #endregion
    }
}
