using FishNet.CodeAnalysis.Annotations;
using FishNet.Connection;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;
using VENTUS.Networking;

namespace VENTUS.Interaction.TransformTool.Core.Selection
{
    public abstract class NetworkInteractionSelectable : LockedOwnership, ISelectable
    {
        protected InteractorGroup InteractorGroup;
    
        private bool IsValidInteractionType => InteractionTracking.CurrentInteractionType == InteractionType;
        protected abstract InteractionTypes InteractionType { get; }
        public virtual bool CanSelect => !HasOwnership && CanTakeOwnership && IsValidInteractionType && !_isSelectionPending && !_isSelectionBlocked;

        private static bool _isSelectionPending;
        private static bool _isSelectionBlocked;
        
        public static void SetSelectionBlocked(bool value)
        {
            _isSelectionBlocked = value;
        }

        [OverrideMustCallBase(BaseCallMustBeFirstStatement = true)]
        public virtual void Select(InteractorGroup interactorGroup)
        {
            InteractorGroup = interactorGroup;
            RequestOwnership();
            _isSelectionPending = true;
        }

        protected sealed override void OnOwnershipAcquired(NetworkConnection prevOwner)
        {
            if (IsValidInteractionType)
            {
                OnSelectOwnershipAcquired(prevOwner);
            }
            
            _isSelectionPending = false;
        }

        protected override void OnOwnershipLost(NetworkConnection prevOwner)
        {
            base.OnOwnershipLost(prevOwner);

            _isSelectionPending = false;
        }

        protected override void OnRequestOwnershipRaceConditionFailed()
        {
            _isSelectionPending = false;
        }

        protected abstract void OnSelectOwnershipAcquired(NetworkConnection prevOwner);
    }
}
