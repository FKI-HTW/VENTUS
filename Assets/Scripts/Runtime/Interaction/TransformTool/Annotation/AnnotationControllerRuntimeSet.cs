using UnityEngine;
using VENTUS.DataStructures.RuntimeSet;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    [CreateAssetMenu(fileName = "new AnnotationControllerRuntimeSet", menuName = "VENTUS/DataStructures/AnnotationControllerRuntimeSet")]
    public class AnnotationControllerRuntimeSet : RuntimeSet<AnnotationController>
    {
        private void OnEnable()
        {
            Restore();
        }
    }
}
