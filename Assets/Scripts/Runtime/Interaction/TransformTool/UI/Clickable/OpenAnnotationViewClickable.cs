using System.Linq;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Annotation;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public class OpenAnnotationViewClickable : ConditionalInstantiateClickable
    {
        [SerializeField] private AnnotationControllerRuntimeSet _annotationControllerRuntimeSet;
        
        protected override bool CanOpen()
        {
            return _annotationControllerRuntimeSet.GetItems().Any(childToParentTransform => childToParentTransform.SelectableGo == SelectableGo);
        }

        protected override void OnConfirmButton()
        {
            SelectionViewStackMachine.PopStackElement();
        }
    }
}
