using UnityEngine;
using UnityEngine.EventSystems;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Interaction.TransformTool.UI.View;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public abstract class InformationClickable : SelectionClickable
    {
        [SerializeField] private ConfirmView _confirmViewPrefab;
        [SerializeField, TextArea(2, 10)] private string _confirmText;
        
        protected override void InternalOnPointerClick(PointerEventData eventData)
        {
            InstantiateInformationView();
        }
        
        private void InstantiateInformationView()
        {
            ConfirmView instantiatedView = Instantiate(_confirmViewPrefab);
            
            foreach (var baseSelectionElement in instantiatedView.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionElement.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
            }
            
            instantiatedView.AddConfirmContext(_confirmText, OnAcceptButton, OnDeclineButton);
        }

        protected abstract void OnAcceptButton();
        protected abstract void OnDeclineButton();
    }
}
