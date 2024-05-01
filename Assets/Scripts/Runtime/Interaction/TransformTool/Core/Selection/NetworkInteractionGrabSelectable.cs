using FishNet;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.InputSystem;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public class NetworkInteractionGrabSelectable : NetworkInteractionSelectable
    {
        #region Fields
        
        [SerializeField] private float _smoothSpeed = 15f;
        [SerializeField] private float _maxDistance = 10f;
        [SerializeField] private bool _snapToHand = true;

        public override bool CanSelect => base.CanSelect && !_isGrabEnabled;
        private bool _isGrabEnabled;

        protected override InteractionTypes InteractionType => InteractionTypes.GrabSelect;
        
        //redo undo data
        private Vector3 _entryPosition;
        private Quaternion _entryRotation;
        private Vector3 _entryScale;

        //grab
        private GrabTransformCalculator _grabTransformCalculator;
        
        #endregion
    
        #region Unity Lifecycle
        
        private void Update()
        {
            if (_isGrabEnabled)
            {
                _grabTransformCalculator.UpdateTransformation(_smoothSpeed);
            }
        }
        
        #endregion
    
        #region Inheritance

        protected override void OnSelectOwnershipAcquired(NetworkConnection prevOwner)
        {
            _isGrabEnabled = true;
            
            Transform thisTransform = transform;
            _entryPosition = thisTransform.position;
            _entryRotation = thisTransform.rotation;
            _entryScale = thisTransform.localScale;
            
            _grabTransformCalculator = new GrabTransformCalculator(InteractorGroup, thisTransform, _maxDistance, _snapToHand);
            InteractionController.InteractorUp += OnInteractionCanceled;
        }
        
        #endregion

        #region Private Methods
        
        private void OnInteractionCanceled(InteractorGroup interactorGroup)
        {
            if (interactorGroup != InteractorGroup) return;

            _isGrabEnabled = false;
            InteractionController.InteractorUp -= OnInteractionCanceled;
            
            Transform thisTransform = transform;
            TransformCommandBroadcastInstaller.BroadcastTransformCommandToServer(gameObject, _entryPosition, thisTransform.position,
                _entryRotation, thisTransform.rotation, _entryScale, thisTransform.localScale);
        
            ReleaseOwnership();
        }
        
        #endregion
    }
}
