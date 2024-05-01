using FishNet.Connection;
using UnityEngine;
using System;
using System.Linq;
using FishNet.Object;
using UnityEngine.Events;

namespace VENTUS.Networking
{
    public class OwnershipObserver : NetworkBehaviour
    {
        [SerializeField] private OwnershipObserverEvents _ownershipObserverEvents;

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            
            UpdateByOwnershipAcquired(Owner.IsValid);
        }
        
        private void UpdateByOwnershipAcquired(bool hasOwner)
        {
            if (hasOwner)
            {
                OnLockOwnershipClient();
                _ownershipObserverEvents.onLockOwnershipClient?.Invoke(Owner);
            }
            else
            {
                OnUnlockOwnershipClient();
                _ownershipObserverEvents.onUnlockOwnershipClient?.Invoke(Owner);
            }
        }
        
        protected virtual void OnLockOwnershipClient() {}
        
        protected virtual void OnUnlockOwnershipClient() {}
        
        public void RegisterAction(UnityAction<NetworkConnection> action, OwnershipObserverEventType firstEventType, params OwnershipObserverEventType[] additionalEventTypes)
        {
            foreach (var ownershipObserverEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (ownershipObserverEventType)
                {
                    case OwnershipObserverEventType.OnLockOwnershipClient:
                        _ownershipObserverEvents.onLockOwnershipClient.AddListener(action);
                        break;
                    case OwnershipObserverEventType.OnUnlockOwnershipClient:
                        _ownershipObserverEvents.onUnlockOwnershipClient.AddListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }
        
        public void UnregisterAction(UnityAction<NetworkConnection> action, OwnershipObserverEventType firstEventType, params OwnershipObserverEventType[] additionalEventTypes)
        {
            foreach (var ownershipObserverEventType in additionalEventTypes.Append(firstEventType))
            {
                switch (ownershipObserverEventType)
                {
                    case OwnershipObserverEventType.OnLockOwnershipClient:
                        _ownershipObserverEvents.onLockOwnershipClient.RemoveListener(action);
                        break;
                    case OwnershipObserverEventType.OnUnlockOwnershipClient:
                        _ownershipObserverEvents.onUnlockOwnershipClient.RemoveListener(action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(additionalEventTypes), additionalEventTypes, null);
                }
            }
        }
        
        [Serializable]
        private class OwnershipObserverEvents
        {
            public UnityEvent<NetworkConnection> onLockOwnershipClient;
            public UnityEvent<NetworkConnection> onUnlockOwnershipClient;
        }
    }

    public enum OwnershipObserverEventType
    {
        OnLockOwnershipClient,
        OnUnlockOwnershipClient
    }
}
