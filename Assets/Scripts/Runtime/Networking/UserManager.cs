using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VENTUS.Controlling;

namespace VENTUS.Networking
{
    public class UserManager : NetworkBehaviour
    {
		#region fields
		
		[SerializeField] private NetworkObject _playerPrefab;
        [SyncObject] private readonly SyncDictionary<int, UserInfo> _userInfo = new();

		public event Action<int> OnUserConnected;
		public event Action<int> OnUserDisconnected;
		public event Action<int> OnUserInfoUpdated;

		public UserInfo LocalUserInfo { get; } = new()
		{
			Name = DefaultValues.NAME,
			Color = DefaultValues.COLOR,
			HairColor = DefaultValues.HAIR_COLOR,
			HairMesh = DefaultValues.HAIR_MESH,
			IsInXRMode = false
		};

		#endregion

		#region lifecycle

		public override void OnStartClient()
		{
			base.OnStartClient();
			_userInfo.OnChange += UserInfoUpdated;
			UpdateUserInfoServer(LocalUserInfo, true);
		}

		public override void OnStopClient()
		{
			base.OnStopClient();
			_userInfo.OnChange -= UserInfoUpdated;
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			InstanceFinder.ServerManager.OnRemoteConnectionState += RemoteConnectionStateChanged;
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			InstanceFinder.ServerManager.OnRemoteConnectionState -= RemoteConnectionStateChanged;
			_userInfo.Clear();
		}
		
		#endregion

		#region private methods

		private void UserInfoUpdated(SyncDictionaryOperation op, int key, UserInfo value, bool asServer)
		{
			if (asServer) return;

			switch (op)
			{
				case SyncDictionaryOperation.Add:
					OnUserConnected?.Invoke(key);
					break;
				case SyncDictionaryOperation.Remove: 
					OnUserDisconnected?.Invoke(key);
					break;
				case SyncDictionaryOperation.Set:
					OnUserInfoUpdated?.Invoke(key);
					break;
				case SyncDictionaryOperation.Clear: break;
				case SyncDictionaryOperation.Complete: break;
			}
		}

		private void RemoteConnectionStateChanged(NetworkConnection conn, RemoteConnectionStateArgs args)
		{
			if (args.ConnectionState == RemoteConnectionState.Started)
			{
				_userInfo.Add(conn.ClientId, new()
				{
					Name = $"Client#{conn.ClientId}",
					Color = DefaultValues.COLOR,
					HairColor = DefaultValues.HAIR_COLOR,
					HairMesh = DefaultValues.HAIR_MESH,
					IsInXRMode = false
				});
			}
			else
			{
				_userInfo.Remove(conn.ClientId);
			}
		}

		[ServerRpc(RequireOwnership = false)]
        private void UpdateUserInfoServer(UserInfo userInfo, bool initialSet, NetworkConnection conn = null)
        {
            if (!IsServer || conn == null) return;
			if (userInfo == null || !_userInfo.ContainsKey(conn.ClientId))
			{
				InstanceFinder.ServerManager.Kick(conn, KickReason.UnusualActivity);
				return;
			}

			_userInfo[conn.ClientId] = userInfo;
			if (!initialSet) return;
			
			var networkManager = InstanceFinder.NetworkManager;
			var nob = networkManager.GetPooledInstantiated(_playerPrefab, _playerPrefab.SpawnableCollectionId, true);
			nob.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			networkManager.ServerManager.Spawn(nob, conn);
			networkManager.SceneManager.AddOwnerToDefaultScene(nob);
        }

		#endregion

		#region public methods

		public void UpdateUserInfo(string username = null, Color? color = null, Color? hairColor = null, 
			EHairCustomization? hairMesh = null, bool? isInXRMode = null)
		{
			LocalUserInfo.Name = username ?? LocalUserInfo.Name;
			LocalUserInfo.Color = color ?? LocalUserInfo.Color;
			LocalUserInfo.HairColor = hairColor ?? LocalUserInfo.HairColor;
			LocalUserInfo.HairMesh = hairMesh ?? LocalUserInfo.HairMesh;
			LocalUserInfo.IsInXRMode = isInXRMode ?? LocalUserInfo.IsInXRMode;
			if (!OnStartClientCalled) return;

			UpdateUserInfoServer(LocalUserInfo, false);
		}

		public List<KeyValuePair<int, UserInfo>> GetAllUserInfos()
		{
			return _userInfo.ToList();
		}

        public bool TryGetUserInfo(NetworkConnection conn, out UserInfo userInfo)
        {
            return _userInfo.TryGetValue(conn.ClientId, out userInfo);
        }
        
        public bool TryGetUserInfo(int clientID, out UserInfo userInfo)
        {
	        return _userInfo.TryGetValue(clientID, out userInfo);
        }

		#endregion
	}

	public class UserInfo
    {
        public string Name;
        public Color Color;
        public Color HairColor;
		public EHairCustomization HairMesh;
		public bool IsInXRMode;

		public override string ToString()
		{
            return Name;
		}
	}
}
