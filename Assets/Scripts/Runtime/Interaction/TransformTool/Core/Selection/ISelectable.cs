using VENTUS.Interaction.Core.Controller;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public interface ISelectable
    {
        public bool CanSelect { get; }
    
        public void Select(InteractorGroup interactorGroup);
    }

    public interface IUnselectable
    {
        public void Unselect();
    }
}
