using UnityEngine;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.DataStructures.CommandLogic.Disposable
{
    public readonly struct DespawnDisposable : ICommandDisposable
    {
        private readonly GameObject _objectToDespawn;

        public DespawnDisposable(GameObject objectToDespawn)
        {
            _objectToDespawn = objectToDespawn;
        }
    
        public void OnDispose()
        {
            if (_objectToDespawn == null || _objectToDespawn.activeSelf) return;
            
            DespawnBroadcastInstaller.DespawnBroadcast(_objectToDespawn);
        }
    }
}
