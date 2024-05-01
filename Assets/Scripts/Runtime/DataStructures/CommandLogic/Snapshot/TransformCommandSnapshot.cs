using FishNet.Connection;
using UnityEngine;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.DataStructures.CommandLogic.Snapshot
{
    public readonly struct TransformCommandSnapshot : ICommandSnapshot
    {
        #region Fields
        private readonly NetworkConnection _networkConnection;
        private readonly Transform _transform;
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _localScale;
        #endregion

        public GameObject RelatedObject => _transform.gameObject;

        #region Constructor
        public TransformCommandSnapshot(NetworkConnection networkConnection, Transform transform, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _networkConnection = networkConnection;
            _transform = transform;
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
        }
        #endregion
        
        #region Public Methods

        public void ApplySnapshot()
        {
            TransformCommandBroadcastInstaller.BroadcastUpdateTransformToClient(_networkConnection, RelatedObject, _position, _rotation, _localScale);
        }
        #endregion
    }
}
