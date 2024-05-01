using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;

namespace VENTUS.Networking
{
    public class DisconnectRemoveOwnership : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
            
            ServerManager.OnRemoteConnectionState += UpdateClientConnectionState;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            
            ServerManager.OnRemoteConnectionState -= UpdateClientConnectionState;
        }

        private void UpdateClientConnectionState(NetworkConnection networkConnection, RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            if (IsDeinitializing) return;
            
            if (remoteConnectionStateArgs.ConnectionState == RemoteConnectionState.Stopped)
            {
                RemoveOwnership();
            }
        }
    }
}
