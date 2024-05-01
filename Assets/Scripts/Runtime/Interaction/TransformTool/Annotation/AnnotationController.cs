using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Snapshot;
using VENTUS.Interaction.TransformTool.Core.Focus;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    public class AnnotationController : NetworkBehaviour, ISelectionFocusElement
    {
        [SerializeField] private AnnotationControllerRuntimeSet _annotationControllerRuntimeSet;
        [SerializeField] private Button _onClickButton;
        [SerializeField] private AnnotationSpectateView _annotationSpectateView;

        [field: SyncVar(Channel = Channel.Reliable, OnChange = nameof(OnChangeTitle))]
        public string Title { get; set; }

        [field: SyncVar(Channel = Channel.Reliable, OnChange = nameof(OnChangeDescription))]
        public string Description { get; set; }
        public GameObject SelectableGo { get; set; }

        private MonoBehaviour _annotationViewPrefabToInstantiate;
        private IAnnotation _instantiatedAnnotationObject;
        private bool _initializedOnSpawnObject;

        #region Unity Lifecycle
    
        private void Awake()
        {
            _annotationViewPrefabToInstantiate = _annotationSpectateView;
            _annotationControllerRuntimeSet.Add(this);
            
            _onClickButton.onClick.AddListener(Open);
        }

        private void OnDisable()
        {
            Close();
        }

        private void OnDestroy()
        {
            _annotationControllerRuntimeSet.Remove(this);
            
            Close();
        }

        #endregion

        #region Inheritance Methods
    
        public void OnApplying(ISelectionFocusElement previous)
        {
            _instantiatedAnnotationObject = Instantiate(_annotationViewPrefabToInstantiate, transform.position, Quaternion.identity).GetComponent<IAnnotation>();
            _instantiatedAnnotationObject.Initialize(this);
        }

        public void OnRestoring()
        {
            _instantiatedAnnotationObject.Dispose();
            _instantiatedAnnotationObject = null;
        }
    
        #endregion
    
        #region Public Methods
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdateValuesServer(NetworkConnection origin, string title, string description)
        {
            //setup previous
            AnnotationDataCommandSnapshot previousSnapshot = new AnnotationDataCommandSnapshot(this);
            
            //apply current -> this also inside snapshot
            Title = title;
            Description = description;
            
            //setup current and push
            ICommandSnapshot currentSnapshot = new AnnotationDataCommandSnapshot(this);
            SnapshotMemento.PushToCommandHandler(origin, SelectableGo, previousSnapshot, currentSnapshot);
        }

        public void Open()
        {
            SelectionFocusSystem.ChangeSelection(typeof(AnnotationController), this);
        }

        public void Close()
        {
            if (SelectionFocusSystem.IsSelected(typeof(AnnotationController), this))
            {
                SelectionFocusSystem.ExitSelection(typeof(AnnotationController));
            }
        }
    
        public void SetViewToSpawn<T>(T newElement) where T : MonoBehaviour, IAnnotation
        {
            _annotationViewPrefabToInstantiate = newElement;
            ExchangeViewOnOpened();
        }
    
        public void SetDefaultViewToSpawn()
        {
            _annotationViewPrefabToInstantiate = _annotationSpectateView;
            ExchangeViewOnOpened();
        }
    
        #endregion

        private void ExchangeViewOnOpened()
        {
            if (_instantiatedAnnotationObject == null) return;
            
            _instantiatedAnnotationObject.Dispose();
            _instantiatedAnnotationObject = Instantiate(_annotationViewPrefabToInstantiate, transform.position, Quaternion.identity).GetComponent<IAnnotation>();
            _instantiatedAnnotationObject.Initialize(this);
        }

        private void OnChangeTitle(string prev, string next, bool asServer)
        {
            _instantiatedAnnotationObject?.OnValuesChanged();
        }
        
        private void OnChangeDescription(string prev, string next, bool asServer)
        {
            _instantiatedAnnotationObject?.OnValuesChanged();
        }
    }
}
