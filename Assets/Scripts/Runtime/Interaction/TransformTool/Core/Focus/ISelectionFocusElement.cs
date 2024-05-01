
namespace VENTUS.Interaction.TransformTool.Core.Focus
{
    public interface ISelectionFocusElement
    {
        void OnApplying(ISelectionFocusElement previous);
        void OnRestoring();
    }
}
