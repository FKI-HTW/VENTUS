using CENTIS.XRPlatformManagement.Controller.Manager;
using CENTIS.XRPlatformManagement.Controller.ProfileBuilding;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace VENTUS.PlaformPackageExtension
{
    public class NetworkedControllerModelSpawner : NetworkBehaviour
    {
        [SerializeField] private ControllerModelSpawnerVariable _controllerModelSpawnerVariable;
        [SerializeField] private Transform _instantiationParent;

        [SyncVar(OnChange = nameof(OnChangeManufacturer))] private string _syncManufacturerType;
        [SyncVar(OnChange = nameof(OnSyncIsEnabled))] private bool _syncIsEnabled;
    
        private string _localManufacturerType;
        private GameObject _localSpawnedController;
        private bool _localSyncIsEnabled;
        private bool _isInitialized;

        private void Awake()
        {
            if (_instantiationParent == null)
            {
                _instantiationParent = transform;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            if (IsOwner && _controllerModelSpawnerVariable.TryGet(out var controllerModelSpawner))
            {
                SetSyncManufacturerType(controllerModelSpawner);
                SetSyncIsEnabled(controllerModelSpawner);
            }
        }
        
        private void SetSyncManufacturerType(ControllerModelSpawner controllerModelSpawner)
        {
            ControllerProfile controllerProfile = controllerModelSpawner.GetCurrentControllerProfile();
            
            string currentManufacturerType = controllerProfile != null ? controllerProfile.ManufacturerType : null;
            if (currentManufacturerType != _localManufacturerType)
            {
                _localManufacturerType = currentManufacturerType;
                ServerSyncManufacturerType(currentManufacturerType);
            }
        }

        private void SetSyncIsEnabled(ControllerModelSpawner controllerModelSpawner)
        {
            bool activeAndEnabled = controllerModelSpawner.isActiveAndEnabled;
            if (activeAndEnabled != _localSyncIsEnabled)
            {
                _localSyncIsEnabled = activeAndEnabled;
                SyncIsEnabled(activeAndEnabled);
            }
        }
    
        [ServerRpc]
        private void ServerSyncManufacturerType(string manufacturerType)
        {
            _syncManufacturerType = manufacturerType;
        }
        
        [ServerRpc]
        private void SyncIsEnabled(bool syncIsEnabled)
        {
            _syncIsEnabled = syncIsEnabled;
        }

        private void OnChangeManufacturer(string prev, string next, bool asServer)
        {
            if (next == null && prev != null)
            {
                Destroy(_localSpawnedController);
            } 
            else if (next != null && prev != next)
            {
                if (prev != null)
                {
                    Destroy(_localSpawnedController);
                }
                
                if (!_controllerModelSpawnerVariable.TryGet(out var controllerModelRenderer))
                    return;

                if (!controllerModelRenderer.TryGetControllerProfileByManufacturerType(next, out var controllerProfile)) 
                    return;
                
                var prefab = controllerModelRenderer
                    .GetControllerModelByProfile(controllerProfile)
                    .GetModelByMask(ControllerModelMask.CompleteModel);
                _localSpawnedController = Instantiate(prefab, _instantiationParent, false);
                _localSpawnedController.AddComponent<ParentLayerInheritor>();
            }
        }

        private void OnSyncIsEnabled(bool prev, bool next, bool asServer)
        {
            if (_localSpawnedController)
            {
                _localSpawnedController.SetActive(next);
            }
        }
    }
}
