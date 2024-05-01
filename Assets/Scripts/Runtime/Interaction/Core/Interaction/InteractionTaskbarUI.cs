using System;
using System.Collections.Generic;
using CENTIS.UnityFileExplorer;
using FishNet;
using FishNet.Managing.Server;
using FishNet.Transporting;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using VENTUS.Controlling;
using VENTUS.Interaction.Sketching;
using VENTUS.Networking;
using VENTUS.ModelImporter;
using VENTUS.UI.Glossary;

namespace VENTUS.Interaction.Core.Interaction
{
    public class InteractionTaskbarUI : MonoBehaviour
	{
		#region fields

        [SerializeField] private SceneManager _sceneManager;
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private UserManager _userManager;
		[SerializeField] private SketchingManager _sketchingManager;
		[SerializeField] private ModelManager _modelManager;

		[SerializeField] private Toggle _glossaryToggle;
		
		[Header("STEP Library")]
		[SerializeField] private Transform _libraryContainer;

		[Header("Explorer Manager")]
		[SerializeField] private ExplorerManager _explorerManager;
		[SerializeField] private Toggle	_explorerToggle;
		[SerializeField] private GameObject	_explorerPanel;

		[Header("Taskbar Text")]
		[SerializeField] private TMP_Text _dateText;
		[SerializeField] private TMP_Text _timeText;
		[SerializeField] private TMP_Text _minimizeText;

		[Header("Minimize Maximize Icons")]
		[SerializeField] private GameObject _minimizeImage;
        [SerializeField] private GameObject _maximizeImage;

        [Header("Taskbar Sections")]
		[SerializeField] private GameObject _topMenus;
		[SerializeField] private GameObject _minimizableButtons;

		[Header("Server Info Panel")] 
		[SerializeField] private Toggle _serverInfoToggle;
		[SerializeField] private TMP_Text _usernameText;
		[SerializeField] private TMP_Text _serverIDText;
		[SerializeField] private TMP_Text _serverHostText;
		[SerializeField] private Transform _connectedClientsPanel;
		[SerializeField] private Transform _connectedClientPrefab;

        private bool _minimized;

		private readonly Dictionary<int, Transform> _connectedClients = new();

		#endregion

		#region lifecycle

		private void Start()
		{
			// update connected client list
			_userManager.OnUserConnected += UserConnected;
			_userManager.OnUserDisconnected += UserDisconnected;
			_userManager.OnUserInfoUpdated += UserInfoUpdated;

			UpdateServerInformation();
			InstanceFinder.ClientManager.OnAuthenticated += UpdateServerInformation;
			InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnected;
			GlossaryManager.OnCurrentEntryUpdated += _ => _glossaryToggle.isOn = true;
			
			_modelManager.LoadLibraryModels(_libraryContainer);
        }

		private void OnDisable()
		{
			_serverInfoToggle.isOn = true;
		}

		private void Update()
		{
			_dateText.text = DateTime.Now.ToString("dd. MMM yy");
			_timeText.text = DateTime.Now.ToString("HH:mm");
		}

		#endregion

		#region public methods

		public void RespawnPlayer()
		{
			_sceneManager.RespawnPlayer();
		}

		public void MinimizeTaskbar()
		{
			_topMenus.SetActive(_minimized);
			_minimizableButtons.SetActive(_minimized);
			
			_minimizeText.text = _minimized ? "Minimize" : "Maximize";
			_minimizeImage.SetActive(_minimized);
			_maximizeImage.SetActive(!_minimized);
			
			_minimized = !_minimized;
        }

        public void CopyServerID()
		{
			GUIUtility.systemCopyBuffer = _networkManager.JoinCode;
		}

		public void EmailServerID()
        {
			const string subject = "VENTUS%20Session%20Invitation%20";
			var body = UnityWebRequest.EscapeURL(
				$"Hello,\n join my VENTUS Session by using the following code: {_networkManager.JoinCode}")
				.Replace(" ", "%20");
			Application.OpenURL($"mailto:?subject={subject}&body={body}");
		}

		public void DisconnectFromServer()
		{
			_networkManager.StopConnection();
		}

		public void SetLineDiameter(float lineDiameter)
		{
			_sketchingManager.LineDiameter = math.remap(0, 1, 0.001f, 0.15f, lineDiameter);
		}

		public void SetSketchingColor(Color color)
		{
			_sketchingManager.CurrentColor = color;
        }

		public void TriggerFileExplorer(bool activate)
        {
            if (activate)
            {
				_explorerManager.FindFile(
					onFilePathFound: OnFileFound, 
					startFolder: Environment.SpecialFolder.Desktop, 
					fileExtensions: new [] { ".stp", ".step", ".gltf", ".glb" }
				);
			}
            else
            {
				_explorerManager.CancelFindFile();
            }
        }

		public void CloseFileExplorer()
        {
			_explorerPanel.SetActive(false);
			_explorerToggle.SetIsOnWithoutNotify(false);
			_explorerManager.CancelFindFile();
		} 

		#endregion

		#region private methods

		private void OnFileFound(string path)
		{
			_modelManager.ImportModel(path);
			_explorerPanel.SetActive(false);
			_explorerToggle.SetIsOnWithoutNotify(false);
		}

		private void UpdateServerInformation()
		{
			_usernameText.text = _networkManager.Username;
			_serverIDText.text = _networkManager.JoinCode;
			_serverHostText.text = _networkManager.IsHost ? "You are the Host!" : "You are not the Host!";

			// enable/disable kick button
			foreach (var trf in _connectedClients.Values)
				trf.GetChild(0).gameObject.SetActive(_networkManager.IsHost);
		}

		private void UserConnected(int clientID)
		{
			if (InstanceFinder.ClientManager.Connection.ClientId == clientID) return;
			if (!_userManager.TryGetUserInfo(clientID, out var userInfo))
			{
				Debug.LogError("UserInfoUpdated for invalid User called!");
				return;
			}

			var connectedClient = Instantiate(_connectedClientPrefab, _connectedClientsPanel);
			connectedClient.GetChild(0).GetComponent<TMP_Text>().text = userInfo.Name;
			
			if (InstanceFinder.IsHost && InstanceFinder.ServerManager.Clients.TryGetValue(clientID, out var conn))
			{
				var button = connectedClient.GetChild(1).GetComponent<Button>();
				button.gameObject.SetActive(true);
				button.onClick.AddListener(() => InstanceFinder.ServerManager.Kick(conn, KickReason.Unset));
			}
			
			_connectedClients.Add(clientID, connectedClient);
		}

		private void UserDisconnected(int clientID)
		{
			if (!_connectedClients.Remove(clientID, out var trf))
				return;
			Destroy(trf.gameObject);
		}

		private void UserInfoUpdated(int clientID)
		{
			if (!_userManager.TryGetUserInfo(clientID, out var userInfo))
			{
				Debug.LogError("UserInfoUpdated for invalid User called!");
				return;
			}

			if (!_connectedClients.TryGetValue(clientID, out var trf)) return;

			trf.GetChild(0).GetComponent<TMP_Text>().text = userInfo.Name;
			trf.GetChild(1).gameObject.SetActive(_networkManager.IsHost);
		}
		
		private void OnClientConnected(ClientConnectionStateArgs args)
		{
			if (args.ConnectionState != LocalConnectionState.Stopped) return;
			
			foreach (var client in _connectedClients.Values)
				Destroy(client.gameObject);
			_connectedClients.Clear();
		}

        #endregion
    }
}
