using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using UnityEngine;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Snapshot;

namespace VENTUS.Networking.BroadcastInstaller
{
    public class TransformCommandBroadcastInstaller : MonoBehaviour
    {
        #region Broadcast Calls

        public static void BroadcastTransformCommandToServer(GameObject relatedObject, Vector3 previousPosition, Vector3 nextPosition, 
            Quaternion previousRotation, Quaternion nextRotation, Vector3 previousScale, Vector3 nextScale)
        {
            TransformCommandBroadcast transformBroadcast = new TransformCommandBroadcast
            {
                relatedObject = relatedObject,
                previousPosition = previousPosition,
                nextPosition = nextPosition,
                previousRotation = previousRotation,
                nextRotation = nextRotation,
                previousScale = previousScale,
                nextScale = nextScale
            };
            
            InstanceFinder.ClientManager.Broadcast(transformBroadcast);
        }
        
        public static void BroadcastUpdateTransformToClient(NetworkConnection networkConnection, GameObject relatedObject, 
            Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
        {
            UpdateTransformBroadcast updateTransformBroadcast = new UpdateTransformBroadcast()
            {
                ownerConnection = networkConnection,
                relatedObject = relatedObject,
                targetPosition = targetPosition,
                targetRotation = targetRotation,
                targetScale = targetScale
            };
            
            InstanceFinder.ServerManager.Broadcast(updateTransformBroadcast);
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            InstanceFinder.ServerManager.RegisterBroadcast<TransformCommandBroadcast>(OnTransformCommand_ServerListenClientBroadcast);
            InstanceFinder.ClientManager.RegisterBroadcast<UpdateTransformBroadcast>(OnUpdateTransform_ClientListenServerBroadcast);
        }

        private void OnDisable()
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<TransformCommandBroadcast>(OnTransformCommand_ServerListenClientBroadcast);
            InstanceFinder.ClientManager.UnregisterBroadcast<UpdateTransformBroadcast>(OnUpdateTransform_ClientListenServerBroadcast);
        }

        #endregion

        #region Private Methods

        private void OnTransformCommand_ServerListenClientBroadcast(NetworkConnection conn, TransformCommandBroadcast transformBroadcast)
        {
            GameObject relatedObject = transformBroadcast.relatedObject;
            
            Transform thisTransform = relatedObject.transform;
            ICommandSnapshot previousSnapshot = new TransformCommandSnapshot(conn, thisTransform, 
                transformBroadcast.previousPosition, transformBroadcast.previousRotation, transformBroadcast.previousScale);
            ICommandSnapshot currentSnapshot = new TransformCommandSnapshot(conn, thisTransform, 
                transformBroadcast.nextPosition, transformBroadcast.nextRotation, transformBroadcast.nextScale);
            SnapshotMemento.PushToCommandHandler(conn, relatedObject, previousSnapshot, currentSnapshot);
        }

        private void OnUpdateTransform_ClientListenServerBroadcast(UpdateTransformBroadcast updateTransformBroadcastBroadcast)
        {
            if (InstanceFinder.ClientManager.Connection != updateTransformBroadcastBroadcast.ownerConnection) return;

            Transform relatedTransform = updateTransformBroadcastBroadcast.relatedObject.transform;
            relatedTransform.position = updateTransformBroadcastBroadcast.targetPosition;
            relatedTransform.rotation = updateTransformBroadcastBroadcast.targetRotation;
            relatedTransform.localScale = updateTransformBroadcastBroadcast.targetScale;
        }

        #endregion
        
        #region Data Structures

        private struct TransformCommandBroadcast : IBroadcast
        {
            public GameObject relatedObject;
            public Vector3 previousPosition;
            public Vector3 nextPosition;
            public Quaternion previousRotation;
            public Quaternion nextRotation;
            public Vector3 previousScale;
            public Vector3 nextScale;
        }

        private struct UpdateTransformBroadcast : IBroadcast
        {
            public NetworkConnection ownerConnection;
            public GameObject relatedObject;
            public Vector3 targetPosition;
            public Quaternion targetRotation;
            public Vector3 targetScale;
        }

        #endregion
    }
}
