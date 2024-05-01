using FishNet.CodeAnalysis.Annotations;
using FishNet.Connection;
using FishNet.Object;

namespace VENTUS.Networking
{
    public class LockedOwnership : NetworkBehaviour
    {
        #region Fields

        public bool CanTakeOwnership => !Owner.IsValid;
        public bool HasOwnership => NetworkObject && IsOwner;
        #endregion
    
        #region Public Methods

        [OverrideMustCallBase(BaseCallMustBeFirstStatement = true)]
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            if (IsOwner)
            {
                OnOwnershipAcquired(prevOwner);
            }
            else
            {
                OnOwnershipLost(prevOwner);
            }
        }

        protected virtual void OnOwnershipAcquired(NetworkConnection prevOwner) { }
        
        protected virtual void OnOwnershipLost(NetworkConnection prevOwner) { }

        [Client]
        public void RequestOwnership()
        {
            //only request it, if there is no owner
            if (!CanTakeOwnership)
                return;
            
            NetworkConnection clientConnection = ClientManager.Connection;
            if (!IsServer)
            {
                ServerTakeOwnership(clientConnection);
            }
            else
            {
                OnTakeOwnership(clientConnection);
            }
        }

        [Client]
        public void ReleaseOwnership()
        {
            if (!IsOwner)
                return;
        
            if (!IsServer)
            {
                ServerReleaseOwnership();
            }
            else
            {
                OnReleaseOwnership();
            }
        }
        #endregion
    
        #region Private Methods

        [ServerRpc(RequireOwnership = false)]
        private void ServerTakeOwnership(NetworkConnection newConnection)
        {
            if (!CanTakeOwnership)
            {
                OnRequestOwnershipRaceConditionFailed();
                return;
            }
            
            OnTakeOwnership(newConnection);
        }

        [Server]
        private void OnTakeOwnership(NetworkConnection newConnection)
        {
            if (!newConnection.IsActive)
                return;
            if (newConnection == Owner)
                return;
        
            GiveOwnership(newConnection);
        }
        
        protected virtual void OnRequestOwnershipRaceConditionFailed() { }

        [ServerRpc(RequireOwnership = true)]
        private void ServerReleaseOwnership()
        {
            OnReleaseOwnership();
        }

        [Server]
        private void OnReleaseOwnership()
        {
            RemoveOwnership();
        }

        #endregion
    }
}
