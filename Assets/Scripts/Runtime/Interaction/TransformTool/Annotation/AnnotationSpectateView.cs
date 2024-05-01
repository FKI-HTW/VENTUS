using TMPro;
using UnityEngine;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    public class AnnotationSpectateView : AnnotationView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;

        protected override void InternalInitialize()
        {
            UpdateContent();
        }
        
        public override void OnValuesChanged()
        {
            UpdateContent();
        }

        private void UpdateContent()
        {
            _title.text = annotationController.Title == string.Empty ? " " : annotationController.Title;
            _description.gameObject.SetActive(!string.IsNullOrEmpty(annotationController.Description));
            _description.text = annotationController.Description;
        }
    }
}
