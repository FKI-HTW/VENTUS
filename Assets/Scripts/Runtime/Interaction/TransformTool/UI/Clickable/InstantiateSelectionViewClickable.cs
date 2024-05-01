using UnityEngine;
using UnityEngine.EventSystems;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public class InstantiateSelectionViewClickable : SelectionClickable
    {
        [SerializeField] private BaseSelectionView _selectionElementPrefab;

        private BaseSelectionView _instantiatedSelectionElementPrefab;

        protected override void InternalOnPointerClick(PointerEventData eventData)
        {
            InstantiateTransformView();
        }

        private void InstantiateTransformView()
        {
            _instantiatedSelectionElementPrefab = Instantiate(_selectionElementPrefab);
            
            foreach (var baseSelectionContext in _instantiatedSelectionElementPrefab.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionContext.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
            }
        }
    }
}
