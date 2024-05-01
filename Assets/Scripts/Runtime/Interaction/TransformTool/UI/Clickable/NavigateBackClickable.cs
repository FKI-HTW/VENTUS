using UnityEngine.EventSystems;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public class NavigateBackClickable : SelectionClickable
    {
        protected override void InternalOnPointerClick(PointerEventData eventData)
        {
            SelectionViewStackMachine.PopStackElement();
        }
    }
}
