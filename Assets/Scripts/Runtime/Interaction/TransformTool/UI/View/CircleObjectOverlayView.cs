using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public class CircleObjectOverlayView : ObjectOverlayView
    {
        #region Fields
        
        [Header("Layout Placement")]
        [SerializeField] private Transform _centerTransform;
        [SerializeField] private float _rotationOffset = 180f;

        protected override Transform InstantiationParent => transform;
        
        private MeshRenderer _selectableMeshRenderer;
        
        #endregion
        
        #region Unity Lifecycle
        
        //TODO: maybe set deeper for upwards compatibility
        protected void Awake()
        {
            if (_centerTransform == null)
            {
                _centerTransform = transform;
            }
        }
        
        #endregion
        
        #region Inheritance
        
        protected override void InstantiateSelectionOptions()
        {
            base.InstantiateSelectionOptions();
            
            PlaceObjectsInCircle();
        }

        #endregion
    
        #region Private Methods
        
        private void PlaceObjectsInCircle()
        {
            for (int instantiatedIndex = 0; instantiatedIndex < instantiatedSelectionOptions.Count; instantiatedIndex++)
            {
                float angle = instantiatedIndex * (360f / instantiatedSelectionOptions.Count) + _rotationOffset;
                
                instantiatedSelectionOptions[instantiatedIndex].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                //revert rotation for all child elements (images/text's should not be rotated - might need changes later)
                for (int childIndex = 0; childIndex < instantiatedSelectionOptions[instantiatedIndex].transform.childCount; childIndex++)
                {
                    instantiatedSelectionOptions[instantiatedIndex].transform.GetChild(childIndex).rotation = transform.rotation;
                }
            }
        }
        
        #endregion
    }
}
