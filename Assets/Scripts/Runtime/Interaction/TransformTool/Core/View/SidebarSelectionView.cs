using System;
using System.Collections;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using VENTUS.UI;
using VENTUS.UI.TransitionView;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class SidebarSelectionView : BaseSelectionView
    {
        [Header("Object Connection")]
        [SerializeField] private TransformConnectionLineRenderer _transformConnectionLineRendererPrefab;

        [Space] 
        [SerializeField] private SidebarSelectionViewEvents _sidebarSelectionViewEvents;

        private TransformConnectionLineRenderer _instantiatedLineRenderer;

        protected override void InternalInitialize()
        {
            base.InternalInitialize();
            
            _sidebarSelectionViewEvents.onInitialize.Invoke();
        }

        public override IEnumerator OnEnter()
        {
            if (_transformConnectionLineRendererPrefab)
            {
                _instantiatedLineRenderer = Instantiate(_transformConnectionLineRendererPrefab);
                _instantiatedLineRenderer.gameObject.SetLayerRecursively(SelectableGo.layer);
                _instantiatedLineRenderer.SetNodes(transform, SelectableGo.transform);
            }
            
            _sidebarSelectionViewEvents.onBeforeEnter.Invoke();
            yield return base.OnEnter();
            _sidebarSelectionViewEvents.onAfterEnter.Invoke();
        }

        public override IEnumerator OnFocus()
        {
            _instantiatedLineRenderer.gameObject.Enable();
            _sidebarSelectionViewEvents.onBeforeFocus.Invoke();
            yield return base.OnFocus();
            _sidebarSelectionViewEvents.onAfterFocus.Invoke();
        }

        public override IEnumerator OnRelease()
        {
            _instantiatedLineRenderer.gameObject.Disable(DeactivationType.Disable);
            _sidebarSelectionViewEvents.onBeforeRelease.Invoke();
            yield return base.OnRelease();
            _sidebarSelectionViewEvents.onAfterRelease.Invoke();
        }

        public override IEnumerator OnExit()
        {
            _instantiatedLineRenderer.gameObject.Disable(DeactivationType.Destroy);
            _instantiatedLineRenderer = null;
        
            _sidebarSelectionViewEvents.onBeforeExit.Invoke();
            yield return base.OnExit();
            _sidebarSelectionViewEvents.onAfterExit.Invoke();
        }
    
        public override void OnExitImmediate()
        {
            _instantiatedLineRenderer.gameObject.Disable(DeactivationType.Destroy);
            _instantiatedLineRenderer = null;
        
            _sidebarSelectionViewEvents.onBeforeExitImmediate.Invoke();
            base.OnExitImmediate();
            _sidebarSelectionViewEvents.onAfterExitImmediate.Invoke();
        }

        public void RegisterAction(UnityAction action, SidebarSelectionViewEventType firstEventType, params SidebarSelectionViewEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
            {
                case SidebarSelectionViewEventType.Initialize:
                    _sidebarSelectionViewEvents.onInitialize.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnBeforeEnter:
                    _sidebarSelectionViewEvents.onBeforeEnter.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnAfterEnter:
                    _sidebarSelectionViewEvents.onAfterEnter.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnBeforeFocus:
                    _sidebarSelectionViewEvents.onBeforeFocus.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnAfterFocus:
                    _sidebarSelectionViewEvents.onAfterFocus.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnBeforeRelease:
                    _sidebarSelectionViewEvents.onBeforeRelease.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnAfterRelease:
                    _sidebarSelectionViewEvents.onAfterRelease.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnBeforeExit:
                    _sidebarSelectionViewEvents.onBeforeExit.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnAfterExit:
                    _sidebarSelectionViewEvents.onAfterExit.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnBeforeExitImmediate:
                    _sidebarSelectionViewEvents.onBeforeExitImmediate.AddListener(action);
                    break;
                case SidebarSelectionViewEventType.OnAfterExitImmediate:
                    _sidebarSelectionViewEvents.onAfterExitImmediate.AddListener(action);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
            }
            }
        }
        
        public void UnregisterAction(UnityAction action, SidebarSelectionViewEventType firstEventType, params SidebarSelectionViewEventType[] additionalEventTypes)
        {
            foreach (var selectionViewEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (selectionViewEventType)
                {
                    case SidebarSelectionViewEventType.Initialize:
                        _sidebarSelectionViewEvents.onInitialize.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnBeforeEnter:
                        _sidebarSelectionViewEvents.onBeforeEnter.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnAfterEnter:
                        _sidebarSelectionViewEvents.onAfterEnter.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnBeforeFocus:
                        _sidebarSelectionViewEvents.onBeforeFocus.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnAfterFocus:
                        _sidebarSelectionViewEvents.onAfterFocus.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnBeforeRelease:
                        _sidebarSelectionViewEvents.onBeforeRelease.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnAfterRelease:
                        _sidebarSelectionViewEvents.onAfterRelease.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnBeforeExit:
                        _sidebarSelectionViewEvents.onBeforeExit.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnAfterExit:
                        _sidebarSelectionViewEvents.onAfterExit.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnBeforeExitImmediate:
                        _sidebarSelectionViewEvents.onBeforeExitImmediate.RemoveListener(action);
                        break;
                    case SidebarSelectionViewEventType.OnAfterExitImmediate:
                        _sidebarSelectionViewEvents.onAfterExitImmediate.RemoveListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }
        
        [Serializable]
        private class SidebarSelectionViewEvents
        {
            public UnityEvent onInitialize;
            public UnityEvent onBeforeEnter;
            public UnityEvent onAfterEnter;
            public UnityEvent onBeforeFocus;
            public UnityEvent onAfterFocus;
            public UnityEvent onBeforeRelease;
            public UnityEvent onAfterRelease; 
            public UnityEvent onBeforeExit;
            public UnityEvent onAfterExit;
            public UnityEvent onBeforeExitImmediate;
            public UnityEvent onAfterExitImmediate;
        }
    }
    
    public enum SidebarSelectionViewEventType
    {
        Initialize, 
        OnBeforeEnter, 
        OnAfterEnter, 
        OnBeforeFocus, 
        OnAfterFocus, 
        OnBeforeRelease, 
        OnAfterRelease,
        OnBeforeExit,
        OnAfterExit,
        OnBeforeExitImmediate,
        OnAfterExitImmediate
    }
}
