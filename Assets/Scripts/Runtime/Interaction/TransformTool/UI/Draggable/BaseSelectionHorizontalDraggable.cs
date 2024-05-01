using System.Linq;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.UI.Draggable
{
    public abstract class BaseSelectionHorizontalDraggable : MonoBehaviour
    {
        #region Fields
        [SerializeField] protected SelectionDraggable selectionDraggable;
        
        [Header("Settings")]
        [SerializeField] private bool _useWorldSpace;
        [SerializeField] private bool _useAxisX;
        [SerializeField] private bool _useAxisY;
        [SerializeField] private bool _useAxisZ;
        [SerializeField] private float _moveFactor = 0.01f;
        [SerializeField] private float _autoRotateAngle = 30f;
    
        private bool _isDragEnabled;
        private Quaternion _previousRotation;
        private Vector3 _entryInteractorRotation;
        
        private Vector3 _entryPosition;
        private Quaternion _entryRotation;
        private Vector3 _entryScale;
        
        #endregion
    
        #region Unity Lifecycle

        private void Awake()
        {
            selectionDraggable.RegisterAction(InternalOnPointerDown, SelectionDraggableEventType.OnPointerDown);
            selectionDraggable.RegisterAction(InternalOnBeginDrag, SelectionDraggableEventType.OnBeginDrag);
            selectionDraggable.RegisterAction(InternalOnEndDrag, SelectionDraggableEventType.OnEndDrag);
        }

        private void OnDestroy()
        {
            selectionDraggable.UnregisterAction(InternalOnPointerDown, SelectionDraggableEventType.OnPointerDown);
            selectionDraggable.UnregisterAction(InternalOnBeginDrag, SelectionDraggableEventType.OnBeginDrag);
            selectionDraggable.UnregisterAction(InternalOnEndDrag, SelectionDraggableEventType.OnEndDrag);
        }
    
        private void Update()
        {
            if (!_isDragEnabled) return;

            Quaternion referenceQuaternion = _previousRotation;
            Quaternion targetQuaternion = selectionDraggable.InteractorGroup.EnabledInteractionInstances.First().Origin.rotation;

            Vector3 referenceEulerAngles = referenceQuaternion.eulerAngles;
            Vector3 targetEulerAngles = targetQuaternion.eulerAngles;

            float deltaYaw;
            float entryDeltaAngle = Mathf.DeltaAngle(_entryInteractorRotation.y, targetEulerAngles.y);
            if (entryDeltaAngle > _autoRotateAngle)
            {
                deltaYaw = 1;
            }
            else if (entryDeltaAngle < -_autoRotateAngle)
            {
                deltaYaw = -1;
            }
            else
            {
                deltaYaw = Mathf.DeltaAngle(referenceEulerAngles.y, targetEulerAngles.y);
            }

            Vector3 finalDirection = Vector3.zero;
            IncrementDirectionVector(_useAxisX, Vector3.right, selectionDraggable.SelectableGo.transform.right, ref finalDirection);
            IncrementDirectionVector(_useAxisY, Vector3.up, selectionDraggable.SelectableGo.transform.up, ref finalDirection);
            IncrementDirectionVector(_useAxisZ, Vector3.forward, selectionDraggable.SelectableGo.transform.forward, ref finalDirection);

            ApplyDirectionVector(finalDirection * deltaYaw * _moveFactor);

            _previousRotation = targetQuaternion;
        }
        
        #endregion

        #region Inheritance
        
        protected abstract void ApplyDirectionVector(Vector3 directionVector);
        
        #endregion

        #region Private Methods
        
        private void InternalOnPointerDown()
        {
            Transform selectableTransform = selectionDraggable.SelectableGo.transform;
            _entryPosition = selectableTransform.position;
            _entryRotation = selectableTransform.rotation;
            _entryScale = selectableTransform.localScale;
            
            _entryInteractorRotation = selectionDraggable.InteractorGroup.EnabledInteractionInstances.First().Origin.rotation.eulerAngles;
        }

        private void InternalOnBeginDrag()
        {
            EnableGrabInteractable();
        }
        
        private void InternalOnEndDrag()
        {
            DisableGrabInteractable();
            
            Transform thisTransform = transform;
            TransformCommandBroadcastInstaller.BroadcastTransformCommandToServer(selectionDraggable.SelectableGo, _entryPosition, thisTransform.position,
                _entryRotation, thisTransform.rotation, _entryScale, thisTransform.localScale);
        }
        
        private void IncrementDirectionVector(bool addMovement, Vector3 worldSpaceVector, Vector3 localSpaceVector, ref Vector3 movementDirection)
        {
            if (addMovement)
            {
                if (_useWorldSpace)
                {
                    movementDirection += worldSpaceVector;
                }
                else
                {
                    movementDirection += localSpaceVector;
                }
            }
        }
    
        private void EnableGrabInteractable()
        {
            _isDragEnabled = true;
        
            _previousRotation = selectionDraggable.InteractorGroup.EnabledInteractionInstances.First().Origin.rotation;
        }

        private void DisableGrabInteractable()
        {
            _isDragEnabled = false;
        }
        
        #endregion
    }
}
