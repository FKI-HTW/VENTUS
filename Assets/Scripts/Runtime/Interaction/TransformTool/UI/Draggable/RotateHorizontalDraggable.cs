using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Draggable
{
    public class RotateHorizontalDraggable : BaseSelectionHorizontalDraggable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionDraggable.SelectableGo.transform.rotation = Quaternion.Euler(directionVector) * selectionDraggable.SelectableGo.transform.rotation;
        }
    }
}
