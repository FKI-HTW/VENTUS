using VENTUS.Interaction.TransformTool.UI.View;
using VENTUS.UI.TransitionView;

namespace VENTUS.UI
{
    public class ErrorPopupView : InformationView
    {
        public void OpenErrorPopupView(string message)
        {
            gameObject.Enable();
            
            AddInformationContext(message, () => gameObject.Disable(DeactivationType.Destroy));
        }
    }
}
