using UnityEngine;
using VENTUS.Interaction.Core.Controller;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public abstract class BaseSelectionElement : MonoBehaviour
    {
        public InteractorGroup InteractorGroup { get; private set; }
        public GameObject SelectableGo { get; private set; }
        public SelectionViewStackMachine SelectionViewStackMachine { get; private set; }
        
        public void Initialize(InteractorGroup interactorGroup, GameObject selectableSystem, SelectionViewStackMachine selectionViewStackMachine)
        {
            InteractorGroup = interactorGroup;
            SelectionViewStackMachine = selectionViewStackMachine;
            SelectableGo = selectableSystem;

            InternalInitialize();
        }

        protected virtual void InternalInitialize() { }
    }
}
