using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;
using VENTUS.Controlling;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Disposable;
using VENTUS.DataStructures.CommandLogic.Snapshot;
using VENTUS.Interaction.TransformTool.Annotation;

namespace VENTUS.Networking.BroadcastInstaller
{
    public class SpawnAnnotationBroadcastInstaller : MonoBehaviour
    {
        #region Broadcast Calls

        public static void BroadcastSpawnToServer(AnnotationController objectToSpawn, Transform parent, Vector3 position, 
            Quaternion rotation, string title, string description)
        {
            SpawnAnnotationBroadcast spawnBroadcast = new SpawnAnnotationBroadcast()
            {
                objectToSpawn = objectToSpawn,
                parent = parent,
                position = position,
                rotation = rotation,
                title = title,
                description = description
            };
            
            InstanceFinder.ClientManager.Broadcast(spawnBroadcast);
        }

        private static void BroadcastSetupToClient(GameObject annotationObject, Transform selectable)
        {
            SetupAnnotationBroadcast setupAnnotationBroadcast = new SetupAnnotationBroadcast()
            {
                annotationObject = annotationObject,
                selectable = selectable
            };
            InstanceFinder.ServerManager.Broadcast(setupAnnotationBroadcast);
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            InstanceFinder.ServerManager.RegisterBroadcast<SpawnAnnotationBroadcast>(OnSpawnAnnotationBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.RegisterBroadcast<SetupAnnotationBroadcast>(OnSetupSelectableBroadcast_ClientListenServer);
        }

        private void OnDisable()
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<SpawnAnnotationBroadcast>(OnSpawnAnnotationBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.UnregisterBroadcast<SetupAnnotationBroadcast>(OnSetupSelectableBroadcast_ClientListenServer);
        }

        #endregion

        #region Private Methods

        private void OnSpawnAnnotationBroadcast_ServerListenClient(NetworkConnection conn, SpawnAnnotationBroadcast spawnBroadcast)
        {
            AnnotationController instantiatedPrefab = Instantiate(spawnBroadcast.objectToSpawn, spawnBroadcast.position, spawnBroadcast.rotation);
            
            instantiatedPrefab.SelectableGo = spawnBroadcast.parent.gameObject;
            instantiatedPrefab.GetComponent<ChildToParentTransform>().ReferenceTransform = spawnBroadcast.parent;
            
            InstanceFinder.ServerManager.Spawn(instantiatedPrefab.gameObject, conn);
            BroadcastSetupToClient(instantiatedPrefab.gameObject, spawnBroadcast.parent);
            instantiatedPrefab.Title = spawnBroadcast.title;
            instantiatedPrefab.Description = spawnBroadcast.description;

            var instantiatedGameObject = instantiatedPrefab.gameObject;
            ICommandSnapshot previousSnapshot = new ActiveAndEnabledSnapshot(instantiatedGameObject, false);
            ICommandSnapshot currentSnapshot = new ActiveAndEnabledSnapshot(instantiatedGameObject, true);
            ICommandDisposable commandDisposable = new DespawnDisposable(instantiatedGameObject);
            SnapshotMemento.PushToCommandHandler(conn, instantiatedPrefab.SelectableGo, previousSnapshot, currentSnapshot, commandDisposable);

            instantiatedPrefab.RemoveOwnership();

		}
        
        private void OnSetupSelectableBroadcast_ClientListenServer(SetupAnnotationBroadcast setupAnnotationBroadcast)
        {
            //TODO: change set selectable gameobject by sync var & figure the other one somehow else
            setupAnnotationBroadcast.annotationObject.GetComponent<AnnotationController>().SelectableGo = setupAnnotationBroadcast.selectable.gameObject;
            setupAnnotationBroadcast.annotationObject.GetComponent<ChildToParentTransform>().ReferenceTransform = setupAnnotationBroadcast.selectable;
        }

        #endregion
        
        #region Data Structures

        private struct SpawnAnnotationBroadcast : IBroadcast
        {
            public AnnotationController objectToSpawn;
            public Transform parent;
            public Vector3 position;
            public Quaternion rotation;
            public string title;
            public string description;
        }

        private struct SetupAnnotationBroadcast : IBroadcast
        {
            public GameObject annotationObject;
            public Transform selectable;
        }

        #endregion
    }
}
