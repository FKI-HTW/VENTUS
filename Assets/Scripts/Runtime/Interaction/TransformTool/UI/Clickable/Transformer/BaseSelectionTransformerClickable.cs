using System.Collections.Generic;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.UI.Clickable.Transformer
{
    public abstract class BaseSelectionTransformerClickable : MonoBehaviour
    {
        #region Fields
        [SerializeField] protected SelectionClickable selectionClickable;
        
        [Header("Setting")]
        [SerializeField] private ClickIntervalDataVariable _clickIntervalDataVariable;
        [SerializeField] private bool _useWorldSpace;
        [SerializeField] private bool _useAxisX;
        [SerializeField] private bool _useAxisY;
        [SerializeField] private bool _useAxisZ;
        [SerializeField] private float _extraScaling;
        
        private float _clickTime;
        private readonly List<ClickIntervalRuntime> _clickIntervalRuntime = new();
        
        private Vector3 _entryPosition;
        private Quaternion _entryRotation;
        private Vector3 _entryScale;
        
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            selectionClickable.RegisterAction(InternalOnPointerDown, SelectionClickableEventType.OnAfterPointerDown);
            selectionClickable.RegisterAction(InternalOnPointerUp, SelectionClickableEventType.OnAfterPointerUp);
        }

        private void OnDestroy()
        {
            selectionClickable.UnregisterAction(InternalOnPointerDown, SelectionClickableEventType.OnAfterPointerDown);
            selectionClickable.UnregisterAction(InternalOnPointerUp, SelectionClickableEventType.OnAfterPointerUp);
        }

        protected virtual void Update()
        {
            if (_extraScaling == 0)
            {
                Debug.LogWarning("The extra scaling is set to 0, resulting in no movement!");
            }

            float clickIntervalDurationSum = 0f;
            for (var index = 0; index < _clickIntervalRuntime.Count; index++)
            {
                var currentClickInterval = _clickIntervalRuntime[index];
                clickIntervalDurationSum += currentClickInterval.durationToPrevious;

                float currentTimeStep = clickIntervalDurationSum;
                float nextTimeStep = Mathf.Infinity;
                if (index + 1 < _clickIntervalRuntime.Count)
                {
                    nextTimeStep = clickIntervalDurationSum + _clickIntervalRuntime[index + 1].durationToPrevious;
                }
            
                if (currentTimeStep <= _clickTime && nextTimeStep > _clickTime)
                {
                    if (!currentClickInterval.CanPerformClick()) break;

                    Vector3 finalDirection = Vector3.zero;
                    IncrementDirectionVector(_useAxisX, Vector3.right, selectionClickable.SelectableGo.transform.right, ref finalDirection);
                    IncrementDirectionVector(_useAxisY, Vector3.up, selectionClickable.SelectableGo.transform.up, ref finalDirection);
                    IncrementDirectionVector(_useAxisZ, Vector3.forward, selectionClickable.SelectableGo.transform.forward, ref finalDirection);
                    ApplyDirectionVector(finalDirection * currentClickInterval.clickStrength * _extraScaling);
                    break;
                }
            }
        
            _clickTime += Time.deltaTime;
        }
        
        #endregion
    
        #region Inheritance
        
        protected abstract void ApplyDirectionVector(Vector3 directionVector);
        
        #endregion
    
        #region Private
        
        private void InternalOnPointerDown()
        {
            Transform selectableTransform = selectionClickable.SelectableGo.transform;;
            _entryPosition = selectableTransform.position;
            _entryRotation = selectableTransform.rotation;
            _entryScale = selectableTransform.localScale;
            
            //click interval
            _clickTime = 0f;
            foreach (var clickIntervalData in _clickIntervalDataVariable.Get())
            {
                _clickIntervalRuntime.Add(new ClickIntervalRuntime(clickIntervalData, false));
            }
        }
        
        private void InternalOnPointerUp()
        {
            Transform thisTransform = transform;
            TransformCommandBroadcastInstaller.BroadcastTransformCommandToServer(selectionClickable.SelectableGo, _entryPosition, thisTransform.position,
                _entryRotation, thisTransform.rotation, _entryScale, thisTransform.localScale);
            
            //click interval
            _clickTime = 0f;
            _clickIntervalRuntime.Clear();
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

        private class ClickIntervalRuntime
        {
            public readonly float durationToPrevious;
            public readonly float clickStrength;
        
            private readonly bool _triggerOnce;
            private bool _hasBeenTriggered;

            public ClickIntervalRuntime(ClickIntervalData clickIntervalData, bool hasBeenTriggered)
            {
                durationToPrevious = clickIntervalData.durationToPrevious;
                clickStrength = clickIntervalData.clickStrength;
                _triggerOnce = clickIntervalData.triggerOnce;
                _hasBeenTriggered = hasBeenTriggered;
            }

            public bool CanPerformClick()
            {
                if (!_triggerOnce) return true;
                if (_hasBeenTriggered) return false;
                _hasBeenTriggered = true;
                return true;
            }
        }
        
        #endregion
    }
}


