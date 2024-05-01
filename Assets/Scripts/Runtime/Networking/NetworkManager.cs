using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using FishNet;
using FishNet.Transporting.UTP;
using VENTUS.Controlling;
using VENTUS.DataStructures;

namespace VENTUS.Networking
{
    public class NetworkManager : MonoBehaviour
    {
		#region properties

		public bool IsOnline => !InstanceFinder.IsOffline;
        public bool IsHost => InstanceFinder.IsHost;
        
        private string _joinCode = string.Empty;
        public string JoinCode => _joinCode;

        private string _playerID = string.Empty;
        public string PlayerID
        {
            get => _playerID;
            private set => _playerID = value;
        }

        private int _maxPlayers = 5;
        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                if (_maxPlayers == value) return;

                if (!IsOnline)
                    _maxPlayers = value;
                else
                    Debug.LogError("Can't change the max number of Players while connected to a Server!");
            }
        }

        private List<Region> _allocationRegions = new();
        public List<Region> AllocationRegions
        {
            get => _allocationRegions;
            private set
            {
                _allocationRegions = value;
				if (_currentAllocationRegion == null || !value.Exists(r => r.Id.Equals(_currentAllocationRegion)))
					_currentAllocationRegion = value.First(r => r.Id.Equals("europe-west4"))?.Id;
			}
        }

        private string _currentAllocationRegion = null;
        public string CurrentAllocationRegion
        {
            get => _currentAllocationRegion;
            set
            {
                if (_currentAllocationRegion == value) return;

                if (IsOnline)
				{
                    Debug.LogError("Can't change the Allocation Region while connected to a Server!");
                    return;
				}

                if (AllocationRegions.Exists(r => r.Id.Equals(value)))
                    _currentAllocationRegion = value;
                else
                    Debug.LogError("The given Allocation Region is not valid!");
			}
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (_username == value) return;

                if (!IsOnline)
                    _username = value;
                else
                    Debug.LogError("Can't change the Username while connected to a Server!");
            }
        }

        private Color _color = DefaultValues.COLOR;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                if (!IsOnline)
                    _color = value;
                else
                    Debug.LogError("Can't change the Color while connected to a Server!");
            }
        }

		private Color _hairColor = DefaultValues.HAIR_COLOR;
		public Color HairColor
		{
			get => _hairColor;
			set
			{
				if (_hairColor == value) return;

				if (!IsOnline)
					_hairColor = value;
				else
					Debug.LogError("Can't change the HairColor while connected to a Server!");
			}
		}

        private EHairCustomization _hairCustomization = DefaultValues.HAIR_MESH;
        public EHairCustomization HairCustomization
        {
            get => _hairCustomization;
			set
			{
				if (_hairCustomization == value) return;

				if (!IsOnline)
					_hairCustomization = value;
				else
					Debug.LogError("Can't change the HairCustomization while connected to a Server!");
			}
		}
        
		#endregion

		#region fields

		[SerializeField] private SceneManager _sceneManager;
		[SerializeField] private UserManager _userManager;
		[SerializeField] private SerializableDictionary<EHairCustomization, Mesh> _hairCustomizationMeshes = new();

        private FishNet.Managing.NetworkManager _networkManager;
        private FishyUnityTransport _transport;

        #endregion

        #region lifecycle

        private void Start()
        {
            _networkManager = GetComponent<FishNet.Managing.NetworkManager>();
            _transport = (FishyUnityTransport)_networkManager.TransportManager.Transport;
        }

		private void OnDisable()
		{
            StopConnection();
		}

		#endregion

		#region public methods

		public async Task<bool> InitializeNetwork()
		{
            await UnityServices.InitializeAsync();
			if (AuthenticationService.Instance.IsSignedIn)
				return true;
            
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("An error occurred trying to sign in the Player. Try again Later!");
                return false;
            }

            PlayerID = AuthenticationService.Instance.PlayerId;
            Debug.Log("The Player was Signed in!");
            return true;
        }

		public async Task<bool> GetRegions()
		{
			if (!AuthenticationService.Instance.IsSignedIn)
				return false;
			AllocationRegions = await RelayService.Instance.ListRegionsAsync();
			return true;
		}

        public async Task<bool> StartHost()
        {
            if (IsOnline)
			{
                Debug.LogError("The local client is already connected to a server!");
                return false;
			}

            if (string.IsNullOrEmpty(Username))
            {
                Debug.LogError("The Username is empty!");
                return false;
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("The Player is not signed in!");
                return false;
            }

            // allocates unity relay server and starts server connection
            var hostAllocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers, CurrentAllocationRegion);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            _transport.SetRelayServerData(new(hostAllocation, "dtls"));
            InstanceFinder.ServerManager.StartConnection();
			InstanceFinder.ClientManager.StartConnection();

			_userManager.UpdateUserInfo(Username, Color, HairColor, HairCustomization, SceneManager.IsInXRMode);

			return true;
        }

        public async Task<bool> JoinAsClient(string joinCode)
        {
            if (IsOnline)
            {
                Debug.LogError("The local client is already connected to a server!");
                return false;
            }

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("The Allocation ID is empty!");
                return false;
            }

            if (string.IsNullOrEmpty(Username))
            {
                Debug.LogError("The Username is empty!");
                return false;
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("The Player is not signed in!");
                return false;
            }

            // retrieves join code from allocation id and starts connection
            _joinCode = joinCode;
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            _transport.SetRelayServerData(new(joinAllocation, "dtls"));
			InstanceFinder.ClientManager.StartConnection();

			_userManager.UpdateUserInfo(Username, Color, HairColor, HairCustomization, SceneManager.IsInXRMode);

			return true;
        }

        public void StopConnection()
        {
            if (InstanceFinder.IsHost)
			{
	            InstanceFinder.ClientManager.StopConnection();
                InstanceFinder.ServerManager.StopConnection(true);
            }
            else
            {
                InstanceFinder.ClientManager.StopConnection();
            }
        }

		public Mesh GetHairMesh(EHairCustomization customization)
		{
			return _hairCustomizationMeshes[customization];
		}

		#endregion
	}
}
