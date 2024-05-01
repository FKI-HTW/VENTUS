using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Clickable.Transformer
{
    public class ScaleClickable : BaseSelectionTransformerClickable
    {
        protected override void ApplyDirectionVector(Vector3 directionVector)
        {
            selectionClickable.SelectableGo.transform.localScale += directionVector;
        }
    }
}
