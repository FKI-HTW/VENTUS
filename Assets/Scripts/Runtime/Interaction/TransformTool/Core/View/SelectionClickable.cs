using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class SelectionClickable : BaseSelectionElement, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private SelectionClickableEvents _selectionClickableEvents;

        protected override void InternalInitialize()
        {
            base.InternalInitialize();
            
            _selectionClickableEvents.onInitialize.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InternalOnPointerClick(eventData);
            _selectionClickableEvents.onAfterPointerClick.Invoke();
        }

        protected virtual void InternalOnPointerClick(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData)
        {
            InternalOnPointerDown(eventData);
            _selectionClickableEvents.onAfterPointerDown.Invoke();
        }

        protected virtual void InternalOnPointerDown(PointerEventData eventData) { }

        public void OnPointerUp(PointerEventData eventData)
        {
            InternalOnPointerUp(eventData);
            _selectionClickableEvents.onAfterPointerUp.Invoke();
        }

        protected virtual void InternalOnPointerUp(PointerEventData eventData) { }
        
        public void RegisterAction(UnityAction action, SelectionClickableEventType firstEventType, params SelectionClickableEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
                {
                    case SelectionClickableEventType.Initialize:
                        _selectionClickableEvents.onInitialize.AddListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerClick:
                        _selectionClickableEvents.onAfterPointerClick.AddListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerDown:
                        _selectionClickableEvents.onAfterPointerDown.AddListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerUp:
                        _selectionClickableEvents.onAfterPointerUp.AddListener(action);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }

        public void UnregisterAction(UnityAction action, SelectionClickableEventType firstEventType,
            params SelectionClickableEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
                {
                    case SelectionClickableEventType.Initialize:
                        _selectionClickableEvents.onInitialize.RemoveListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerClick:
                        _selectionClickableEvents.onAfterPointerClick.RemoveListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerDown:
                        _selectionClickableEvents.onAfterPointerDown.RemoveListener(action);
                        break;
                    case SelectionClickableEventType.OnAfterPointerUp:
                        _selectionClickableEvents.onAfterPointerUp.RemoveListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }


        [Serializable]
        private class SelectionClickableEvents
        {
            public UnityEvent onInitialize;
            public UnityEvent onAfterPointerClick;
            public UnityEvent onAfterPointerDown;
            public UnityEvent onAfterPointerUp;
        }
    }
    
    public enum SelectionClickableEventType
    {
        Initialize,
        OnAfterPointerClick, 
        OnAfterPointerDown, 
        OnAfterPointerUp
    }
}
