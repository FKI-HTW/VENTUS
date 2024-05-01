using FishNet.Connection;
using UnityEngine;
using VENTUS.Interaction.Core.Interaction;
using VENTUS.Interaction.TransformTool.Core.Focus;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public sealed class NetworkInteractionTransformSelectable : NetworkInteractionSelectable, ISelectionFocusElement, IInteractionActivatable
    {
        #region Fields

        [SerializeField] private BaseSelectionView _baseSelectionView;
    
        private BaseSelectionView _instantiatedTransformToolView;
        private SelectionViewStackMachine _selectionViewStackMachine;
        
        protected override InteractionTypes InteractionType => InteractionTypes.TransformSelect;
        public bool IsSelected { get; private set; }

        #endregion

        #region Unity Lifecycle
        
        private void Awake()
        {
            //registration for both interfaces
            InteractionTracking.AddElement(InteractionType, this);
            _selectionViewStackMachine = new SelectionViewStackMachine(this);
        }

        private void OnDisable()
        {
            Close();
        }

        private void OnDestroy()
        {
            Close();
            //unsubscription for both interfaces
            InteractionTracking.RemoveElement(InteractionType, this);
        }
        
        #endregion
    
        #region Inheritance
    
        protected override void OnSelectOwnershipAcquired(NetworkConnection prevOwner)
        {
            SelectionFocusSystem.ChangeSelection(GetType(), this);
        }
        
        #endregion

        #region Public Methods
        
        public void OnEnableInteraction() { }

        public void OnDisableInteraction()
        {
            Close();
        }
        
        public void Close()
        {
            if (SelectionFocusSystem.IsSelected(GetType(), this))
            {
                SelectionFocusSystem.ExitSelection(GetType());
            }
        }
    
        public void OnApplying(ISelectionFocusElement previous)
        {
            IsSelected = true;
            _instantiatedTransformToolView = Instantiate(_baseSelectionView);
            _instantiatedTransformToolView.Initialize(InteractorGroup, gameObject, _selectionViewStackMachine);
        }

        public void OnRestoring()
        {
            IsSelected = false;
            
            // Given the mutual ability of the SelectionFocusSystem and the SelectionViewStackMachine to exit each other,
            // it's essential to ensure that each one performs this operation exactly once.
            if (_selectionViewStackMachine is { HasStack: true })
            {
                _selectionViewStackMachine.ClearStack();
            }
        }
        
        #endregion
    }
}
