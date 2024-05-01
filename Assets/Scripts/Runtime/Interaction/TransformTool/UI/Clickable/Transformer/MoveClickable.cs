using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Clickable.Transformer
{
    public class MoveClickable : BaseSelectionTransformerClickable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionClickable.SelectableGo.transform.position += directionVector;
        }
    }
}
