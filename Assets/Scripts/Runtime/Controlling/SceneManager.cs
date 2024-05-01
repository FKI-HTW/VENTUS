using FishNet;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;
using System.Collections.Generic;
using VENTUS.Networking;
using VENTUS.UI;

namespace VENTUS.Controlling
{
    public class SceneManager : MonoBehaviour
    {
        #region fields

        [SerializeField] private UserManager _userManager;
        [SerializeField] private Canvas _lobbyUI;
        [SerializeField] private UIServerSelection _serverSelection;
        [SerializeField] private List<Canvas> _gameUis = new();
        
        [Header("Desktop")]
        [SerializeField] private GameObject _desktopPlatform;
        [SerializeField] private DesktopPlayerController _desktopController;

        [Header("XR")] 
        [SerializeField] private GameObject _XRPlatform;
        [SerializeField] private TeleportationArea _teleportArea;

        public static bool IsInXRMode { get; private set; }

        #endregion
        
        #region lifecycle
        
        private void Start()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
            
            IsInXRMode = false;
            _desktopPlatform.SetActive(true);
            _lobbyUI.renderMode = RenderMode.ScreenSpaceCamera;
            _userManager.UpdateUserInfo(isInXRMode: false);
            
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnected;
        }

        private void OnDestroy()
        {
            var clientManager = InstanceFinder.ClientManager;
            if (clientManager)
                clientManager.OnClientConnectionState -= OnClientConnected;
        }

        private void OnApplicationQuit()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }

        #endregion
        
        #region public methods
        
        public void RespawnPlayer(bool enableMovement = true)
        {
            if (IsInXRMode)
            {
                _teleportArea.teleportationProvider.QueueTeleportRequest(new() {
                    destinationPosition = Vector3.zero,
                    matchOrientation = MatchOrientation.None
                });
            }
            else
            {
                _desktopController.ResetPlayer();
            }
            
            _teleportArea.enabled = enableMovement;
            _desktopController.enabled = enableMovement;
        }

        public bool StartXRScene()
        {
            if (IsInXRMode) return true;
            
            Debug.Log("Starting XR...");
            
            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Starting XR Failed. Check Editor or Player log for details.");
                return false;
            }
            
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            
            _desktopPlatform.SetActive(false);
            IsInXRMode = true;
            _XRPlatform.SetActive(true);
            _lobbyUI.renderMode = RenderMode.WorldSpace;
            _userManager.UpdateUserInfo(isInXRMode: true);
            
            Debug.Log("XR Started.");
            return true;
        }

        public bool StartDesktopScene()
        {
            if (!IsInXRMode) return true;
            
            Debug.Log("Starting Desktop...");

            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
            _XRPlatform.SetActive(false);
            
            IsInXRMode = false;
            _desktopPlatform.SetActive(true);
            _lobbyUI.renderMode = RenderMode.ScreenSpaceCamera;
            _userManager.UpdateUserInfo(isInXRMode: false);
            
            Debug.Log("Started Desktop.");
            return true;
        }

        public void SwitchScene()
        {
            if (IsInXRMode)
                StartDesktopScene();
            else
                StartXRScene();
        }
        
        #endregion
        
        #region private methods

        private void OnClientConnected(ClientConnectionStateArgs args)
        {
            switch (args.ConnectionState)
            {
                case LocalConnectionState.Started:
                    _lobbyUI.gameObject.SetActive(false);
                    _serverSelection.ShowWelcomeWindow();
                    foreach (var ui in _gameUis)
                        ui.gameObject.SetActive(true);
                    RespawnPlayer(true);
                    break;
                case LocalConnectionState.Stopped:
                    foreach (var ui in _gameUis)
                        ui.gameObject.SetActive(false);
                    _lobbyUI.gameObject.SetActive(true);
                    RespawnPlayer(false);
                    break;
                case LocalConnectionState.Starting: break;
                case LocalConnectionState.Stopping: break;
            }
        }
    
        #endregion
    }
}
