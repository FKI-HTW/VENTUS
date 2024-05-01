using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;
using UnityEngine.Serialization;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Disposable;
using VENTUS.DataStructures.CommandLogic.Snapshot;

namespace VENTUS.Networking.BroadcastInstaller
{
    public class ActiveStateBroadcastInstaller : MonoBehaviour
    {
        #region Broadcast Calls

        public static void BroadcastActiveStateToServer(GameObject[] gameObject, bool isActive, bool addDespawnDisposable)
        {
            ActiveStateToServerBroadcast activeStateToServerBroadcast = new ActiveStateToServerBroadcast()
            {
                relatedObjects = gameObject,
                isActive = isActive,
                addDespawnDisposable = addDespawnDisposable
            };
            InstanceFinder.ClientManager.Broadcast(activeStateToServerBroadcast);
        }
        
        public static void BroadcastActiveStateToClient(GameObject gameObject, bool isActive)
        {
            ActiveStateToClientBroadcast activeStateBroadcast = new ActiveStateToClientBroadcast()
            {
                relatedObject = gameObject,
                isActive = isActive,
            };
            InstanceFinder.ServerManager.Broadcast(activeStateBroadcast);
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            InstanceFinder.ServerManager.RegisterBroadcast<ActiveStateToServerBroadcast>(OnActiveStateBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.RegisterBroadcast<ActiveStateToClientBroadcast>(OnActiveStateBroadcast_ClientListenServer);
        }

        private void OnDisable()
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<ActiveStateToServerBroadcast>(OnActiveStateBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.UnregisterBroadcast<ActiveStateToClientBroadcast>(OnActiveStateBroadcast_ClientListenServer);
        }

        #endregion

        #region Private Methods

        private void OnActiveStateBroadcast_ServerListenClient(NetworkConnection conn, ActiveStateToServerBroadcast activeStateToServerBroadcast)
        {
            GameObject[] relatedObjects = activeStateToServerBroadcast.relatedObjects;
            bool isActive = activeStateToServerBroadcast.isActive;
            
            foreach (var relatedObject in relatedObjects)
            {
                relatedObject.SetActive(isActive);
                ActiveStateToClientBroadcast activeStateToClientBroadcast = new ActiveStateToClientBroadcast()
                {
                    relatedObject = relatedObject,
                    isActive = isActive
                };
                InstanceFinder.ServerManager.Broadcast(activeStateToClientBroadcast);
            }

            if (InstanceFinder.IsServer)
            {
                ICommandSnapshot[] previousSnapshots = new ICommandSnapshot[relatedObjects.Length];
                ICommandSnapshot[] currentSnapshots = new ICommandSnapshot[relatedObjects.Length];
                ICommandDisposable[] despawnDisposables = new ICommandDisposable[relatedObjects.Length];
                
                for (var i = 0; i < relatedObjects.Length; i++)
                {
                    GameObject relatedObject = relatedObjects[i];
                    previousSnapshots[i] = new ActiveAndEnabledSnapshot(relatedObject, !isActive);
                    currentSnapshots[i] = new ActiveAndEnabledSnapshot(relatedObject, isActive);
                    despawnDisposables[i] = new DespawnDisposable(relatedObject);
                }
                
                if (activeStateToServerBroadcast.addDespawnDisposable)
                {
                    SnapshotMemento.PushToCommandHandler(conn, relatedObjects, previousSnapshots, currentSnapshots, despawnDisposables);
                }
            }
        }
        
        private void OnActiveStateBroadcast_ClientListenServer(ActiveStateToClientBroadcast activeStateBroadcast)
        {
            activeStateBroadcast.relatedObject.SetActive(activeStateBroadcast.isActive);
        }

        #endregion

        #region Data Structures

        private struct ActiveStateToServerBroadcast : IBroadcast
        {
            public GameObject[] relatedObjects;
            public bool isActive;
            public bool addDespawnDisposable;
        }

        private struct ActiveStateToClientBroadcast : IBroadcast
        {
            public GameObject relatedObject;
            public bool isActive;
        }

        #endregion
    }
}
