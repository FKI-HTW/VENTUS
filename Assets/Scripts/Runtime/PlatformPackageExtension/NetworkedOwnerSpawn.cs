using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace VENTUS.PlatformPackageExtension
{
    public class NetworkedOwnerSpawn : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _prefabToSpawn;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                RequestNetworkSpawn(_prefabToSpawn, Owner);
            }
        }

        [ServerRpc]
        private void RequestNetworkSpawn(NetworkObject objectToInstantiate, NetworkConnection newOwner)
        {
            NetworkObject instantiatedPrefab = Instantiate(objectToInstantiate, transform, false);
            ServerManager.Spawn(instantiatedPrefab, newOwner);
        }
    }
}
