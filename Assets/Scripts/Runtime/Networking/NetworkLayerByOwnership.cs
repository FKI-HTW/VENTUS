using UnityEngine;
using VENTUS.UI;

namespace VENTUS.Networking
{
    public class NetworkLayerByOwnership : OwnershipObserver
    {
        [SerializeField] private SingleUnityLayer _defaultLayer;
        [SerializeField] private SingleUnityLayer _ownerOwnershipLayer;
        [SerializeField] private SingleUnityLayer _remoteOwnershipLayer;
        
        private void SetLayer()
        {
            gameObject.layer = IsOwner ? _ownerOwnershipLayer.LayerIndex : _remoteOwnershipLayer.LayerIndex;
        }

        private void RestoreLayer()
        {
            gameObject.layer = _defaultLayer.LayerIndex;
        }

        protected override void OnLockOwnershipClient()
        {
            SetLayer();
        }

        protected override void OnUnlockOwnershipClient()
        {
            RestoreLayer();
        }
    }
}