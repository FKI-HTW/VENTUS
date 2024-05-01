using UnityEngine;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.DataStructures.CommandLogic.Snapshot
{
    public readonly struct ActiveAndEnabledSnapshot : ICommandSnapshot
    {
        private readonly GameObject _relatedObject;
        private readonly bool _isActive;

        public GameObject RelatedObject => _relatedObject.gameObject;

        public ActiveAndEnabledSnapshot(GameObject relatedObject, bool isActive)
        {
            _relatedObject = relatedObject;
            _isActive = isActive;
        }

        public void ApplySnapshot()
        {
            ActiveStateBroadcastInstaller.BroadcastActiveStateToClient(_relatedObject, _isActive);
        }
    }
}
