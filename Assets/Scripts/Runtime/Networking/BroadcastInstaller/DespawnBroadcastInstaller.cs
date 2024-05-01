using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;

namespace VENTUS.Networking.BroadcastInstaller
{
    public class DespawnBroadcastInstaller : MonoBehaviour
    {
        #region Broadcast Calls

        public static void DespawnBroadcast(GameObject objectToDespawn)
        {
            Despawn_ServerListenClientBroadcast despawnBroadcast = new Despawn_ServerListenClientBroadcast()
            {
                relatedObject = objectToDespawn
            };
        
            InstanceFinder.ClientManager.Broadcast(despawnBroadcast);
        }

        #endregion
    
        #region Unity Lifecycle

        private void OnEnable()
        {
            InstanceFinder.ServerManager.RegisterBroadcast<Despawn_ServerListenClientBroadcast>(OnDespawnBroadcast_ServerListenClient);
        }

        private void OnDisable()
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<Despawn_ServerListenClientBroadcast>(OnDespawnBroadcast_ServerListenClient);
        }

        #endregion
    
        #region Private Methods

        private void OnDespawnBroadcast_ServerListenClient(NetworkConnection conn, Despawn_ServerListenClientBroadcast despawnBroadcast)
        {
            InstanceFinder.ServerManager.Despawn(despawnBroadcast.relatedObject);
        }

        #endregion

        #region Data Structures

        private struct Despawn_ServerListenClientBroadcast : IBroadcast
        {
            public GameObject relatedObject;
        }

        #endregion
    }
}
