using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Draggable
{
    public class ScaleHorizontalDraggable : BaseSelectionHorizontalDraggable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionDraggable.SelectableGo.transform.localScale += directionVector;
        }
    }
}
