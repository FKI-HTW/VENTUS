using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Clickable.Transformer
{
    public class RotateClickable : BaseSelectionTransformerClickable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionClickable.SelectableGo.transform.rotation = Quaternion.Euler(directionVector) * selectionClickable.SelectableGo.transform.rotation;
        }
    }
}
