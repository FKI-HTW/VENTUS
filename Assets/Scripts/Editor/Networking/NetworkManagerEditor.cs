using UnityEngine;
using UnityEditor;
using VENTUS.Controlling;

namespace VENTUS.Networking
{
	[CustomEditor(typeof(NetworkManager))]
	public class NetworkManagerEditor : Editor
	{
		private NetworkManager _manager;

		private string _joinCode = "";
	
		private int _regionIndex = 0;
		private int RegionIndex
		{
			get => _regionIndex;
			set
			{
				if (_regionIndex != value && _manager.AllocationRegions.Count - 1 > value)
				{
					_regionIndex = value;
					_manager.CurrentAllocationRegion = _manager.AllocationRegions[value].Id;
				}
			}
		}

		private void OnEnable()
		{
			_manager = (NetworkManager)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Separator();

			if (_manager.IsOnline)
				OnlineGUI();
			else
				OfflineGUI();
		}

		private void OnlineGUI()
		{
			SelectableLabel("Join Code:", _manager.JoinCode, true);
			SelectableLabel("Username:", _manager.Username);
			EditorGUILayout.Toggle("Is Host:", _manager.IsHost);
			EditorGUILayout.Separator();

			if (GUILayout.Button(new GUIContent("Stop Connection")))
				_manager.StopConnection();
		}

		private async void OfflineGUI()
		{
			if (GUILayout.Button("Initialize"))
			{
				await _manager.InitializeNetwork();
				await _manager.GetRegions();
			}
			
			SelectableLabel("Player ID:", _manager.PlayerID);
			_manager.Username = EditorGUILayout.TextField("Username:", _manager.Username);
			_manager.Color = EditorGUILayout.ColorField("Color:", _manager.Color);
			_manager.HairColor = EditorGUILayout.ColorField("HairColor:", _manager.HairColor);
			_manager.HairCustomization = (EHairCustomization)EditorGUILayout.EnumPopup("HairCustomization:", _manager.HairCustomization);
			EditorGUILayout.Separator();

			GUILayout.Label("Host", EditorStyles.boldLabel);
			{	// max players
				_manager.MaxPlayers = EditorGUILayout.IntField("Max Players:", _manager.MaxPlayers);
			}
			{   // region list
				string[] regionNames = new string[_manager.AllocationRegions.Count];
				for (int i = 0; i < _manager.AllocationRegions.Count; i++)
				{
					if (_manager.AllocationRegions[i].Id.Equals(_manager.CurrentAllocationRegion))
						RegionIndex = i;
					regionNames[i] = _manager.AllocationRegions[i].Description;
				}
				RegionIndex = EditorGUILayout.Popup("Region:", RegionIndex, regionNames);
			}
			{	// start host
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Create")))
					_ = _manager.StartHost();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Separator();

			GUILayout.Label("Client", EditorStyles.boldLabel);
			{	// server id
				_joinCode = EditorGUILayout.TextField("Join Code: ", _joinCode);
			}
			{	// join server
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Join")))
					_ = _manager.JoinAsClient(_joinCode);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void SelectableLabel(string label, string value, bool copy = false)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
			EditorGUILayout.SelectableLabel(value, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
			if (copy)
			{
				if (GUILayout.Button("Copy"))
					GUIUtility.systemCopyBuffer = value;
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
