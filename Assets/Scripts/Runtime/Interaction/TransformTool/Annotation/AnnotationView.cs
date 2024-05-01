using Unity.XR.CoreUtils;
using UnityEngine;
using VENTUS.UI;
using VENTUS.UI.TransitionView;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    public abstract class AnnotationView : MonoBehaviour, IAnnotation
    {
        #region Fields
        [Header("Line Renderer")]
        [SerializeField] private TransformConnectionLineRenderer _transformConnectionLineRendererPrefab;

        protected AnnotationController annotationController;
    
        private TransformConnectionLineRenderer _instantiatedTransformConnectionLineRenderer;
        
        #endregion
    
        #region Inheritance Methods
    
        public void Initialize(AnnotationController annotationController)
        {
            this.annotationController = annotationController;

            InitializeConnectionLineRenderer();
            gameObject.Enable();
        
            InternalInitialize();
        }

        public abstract void OnValuesChanged();

        protected abstract void InternalInitialize();

        public void Dispose()
        {
            DisposeConnectionLineRenderer();
            gameObject.Disable(DeactivationType.Destroy);
        }
    
        #endregion

        private void InitializeConnectionLineRenderer()
        {
            _instantiatedTransformConnectionLineRenderer = Instantiate(_transformConnectionLineRendererPrefab);
            _instantiatedTransformConnectionLineRenderer.gameObject.SetLayerRecursively(this.annotationController.SelectableGo.layer);
            _instantiatedTransformConnectionLineRenderer.SetNodes(transform, this.annotationController.transform);
        }
        
        private void DisposeConnectionLineRenderer()
        {
            _instantiatedTransformConnectionLineRenderer.RemoveNodes();
            _instantiatedTransformConnectionLineRenderer.gameObject.Disable(DeactivationType.Destroy);
            _instantiatedTransformConnectionLineRenderer = null;
        }
    
        #region Public Methods
    
        public void Close()
        {
            annotationController.Close();
        }
    
        #endregion
    }
}
