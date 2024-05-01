using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Draggable
{
    public class MoveHorizontalDraggable : BaseSelectionHorizontalDraggable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionDraggable.SelectableGo.transform.position += directionVector;
        }
    }
}
