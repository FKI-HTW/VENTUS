using UnityEngine;
using UnityEngine.EventSystems;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Interaction.TransformTool.UI.View;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public abstract class ConditionalInstantiateClickable : SelectionClickable
    {
        [Header("Condition")]
        [SerializeField] private InformationView _informationViewPrefab;
        [SerializeField] private string _informationText;
    
        [Header("View")]
        [SerializeField] private BaseSelectionView _selectionElementPrefab;

        private BaseSelectionView _instantiatedSelectionElementPrefab;
        
        protected override void InternalOnPointerClick(PointerEventData eventData)
        {
            if (CanOpen())
            {
                InstantiateTransformView();
            }
            else
            {
                InstantiateInformationView();
            }
        }

        protected virtual void InstantiateInformationView()
        {
            InformationView instantiatedConfirmView = Instantiate(_informationViewPrefab);
            
            foreach (var baseSelectionContext in instantiatedConfirmView.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionContext.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
            }
            
            instantiatedConfirmView.AddInformationContext(_informationText, OnConfirmButton);
        }
    
        protected virtual void InstantiateTransformView()
        {
            _instantiatedSelectionElementPrefab = Instantiate(_selectionElementPrefab);
            
            foreach (var baseSelectionContext in _instantiatedSelectionElementPrefab.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionContext.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
            }
        }

        protected abstract bool CanOpen();
        protected abstract void OnConfirmButton();
    }
}
