using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace VENTUS.DataStructures.CommandLogic
{
    public class CommandNetworkObject : NetworkBehaviour
    {
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);

            SetCommandOwnership(Owner);
        }

        public void SetCommandOwnership(NetworkConnection owner)
        {
			if (!owner.IsValid) return;
            
            foreach (var commandInstance in CommandHandler.CommandInstances)
            {
                if (commandInstance.Owner != owner)
                {
                    commandInstance.RemoveCommandsByIdentifier(gameObject);
                }
            }
        }

        public override void OnDespawnServer(NetworkConnection connection)
        {
            base.OnDespawnServer(connection);
            
            foreach (CommandInstance commandHandler in CommandHandler.CommandInstances)
            {
                commandHandler.RemoveCommandsByIdentifier(gameObject);
            }
        }
    }
}
