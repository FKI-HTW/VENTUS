using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class SelectionDraggable : BaseSelectionElement, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SelectionDraggableEvents _selectionDraggableEvents;
        
        #region Unity Lifecycle

        public void OnPointerDown(PointerEventData eventData)
        {
            InternalOnPointerDown(eventData);
            _selectionDraggableEvents.onPointerDown.Invoke();
        }

        protected virtual void InternalOnPointerDown(PointerEventData eventData) { }

        public void OnBeginDrag(PointerEventData eventData)
        {
            InternalOnBeginDrag(eventData);
            _selectionDraggableEvents.onBeginDrag.Invoke();
        }

        protected virtual void InternalOnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            InternalOnDrag(eventData);
            _selectionDraggableEvents.onDrag.Invoke();
        }

        protected virtual void InternalOnDrag(PointerEventData eventData) { }

        public void OnEndDrag(PointerEventData eventData)
        {
            InternalOnEndDrag(eventData);
            _selectionDraggableEvents.onEndDrag.Invoke();
        }

        protected virtual void InternalOnEndDrag(PointerEventData eventData) { }
        
        #endregion
        
        public void RegisterAction(UnityAction action, SelectionDraggableEventType firstEventType, params SelectionDraggableEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
                {
                    case SelectionDraggableEventType.Initialize:
                        _selectionDraggableEvents.onInitialize.AddListener(action);
                        break;
                    case SelectionDraggableEventType.OnPointerDown:
                        _selectionDraggableEvents.onPointerDown.AddListener(action);
                        break;
                    case SelectionDraggableEventType.OnBeginDrag:
                        _selectionDraggableEvents.onBeginDrag.AddListener(action);
                        break;
                    case SelectionDraggableEventType.OnDrag:
                        _selectionDraggableEvents.onDrag.AddListener(action);
                        break;
                    case SelectionDraggableEventType.OnEndDrag:
                        _selectionDraggableEvents.onEndDrag.AddListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }

        public void UnregisterAction(UnityAction action, SelectionDraggableEventType firstEventType,
            params SelectionDraggableEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
                {
                    case SelectionDraggableEventType.Initialize:
                        _selectionDraggableEvents.onInitialize.RemoveListener(action);
                        break;
                    case SelectionDraggableEventType.OnPointerDown:
                        _selectionDraggableEvents.onPointerDown.RemoveListener(action);
                        break;
                    case SelectionDraggableEventType.OnBeginDrag:
                        _selectionDraggableEvents.onBeginDrag.RemoveListener(action);
                        break;
                    case SelectionDraggableEventType.OnDrag:
                        _selectionDraggableEvents.onDrag.RemoveListener(action);
                        break;
                    case SelectionDraggableEventType.OnEndDrag:
                        _selectionDraggableEvents.onEndDrag.RemoveListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }

        [Serializable]
        private class SelectionDraggableEvents
        {
            public UnityEvent onInitialize;
            public UnityEvent onPointerDown;
            public UnityEvent onBeginDrag;
            public UnityEvent onDrag;
            public UnityEvent onEndDrag;
        }
    }
    
    public enum SelectionDraggableEventType
    {
        Initialize,
        OnPointerDown, 
        OnBeginDrag, 
        OnDrag, 
        OnEndDrag
    }
}
