using UnityEngine;
using VENTUS.Interaction.TransformTool.Annotation;

namespace VENTUS.DataStructures.CommandLogic.Snapshot
{
    public readonly struct AnnotationDataCommandSnapshot : ICommandSnapshot
    {
        private readonly AnnotationController _annotationController;
        private readonly string _title;
        private readonly string _description;
        
        public GameObject RelatedObject => _annotationController.gameObject;

        public AnnotationDataCommandSnapshot(AnnotationController annotationController)
        {
            _annotationController = annotationController;
            _title = _annotationController.Title;
            _description = _annotationController.Description;
        }

        public void ApplySnapshot()
        {
            _annotationController.Title = _title;
            _annotationController.Description = _description;
        }
    }
}
