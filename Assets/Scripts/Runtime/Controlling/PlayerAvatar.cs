using System;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using VENTUS.DataStructures.Variables;
using VENTUS.Networking;

namespace VENTUS.Controlling
{
	public class PlayerAvatar : NetworkBehaviour
	{
		#region fields

		[Header("XR Position References")]
		[SerializeField] private CameraVariable _cameraVariable;
		[SerializeField] private TransformVariable _leftHandTransformVariable;
		[SerializeField] private TransformVariable _rightHandTransformVariable;

		[Header("Child Object References")]
		[SerializeField] private Transform	_head;
		[SerializeField] private Transform	_leftHand;
		[SerializeField] private GameObject	_leftHandVisual;
		[SerializeField] private Transform	_rightHand;
		[SerializeField] private GameObject	_rightHandVisual;
		
		[SerializeField] private Canvas _canvas;
		[SerializeField] private TMP_Text _username;
		[SerializeField] private Vector3 _usernameOffset = new(0, 0.6f, 0);

		[Header("Avatar References")]
		[SerializeField] private GameObject _avatar;
		[SerializeField] private SkinnedMeshRenderer _shirtRenderer;
		[SerializeField] private Material _shirtMaterial;
		[SerializeField] private SkinnedMeshRenderer _hairRenderer;
		[SerializeField] private SkinnedMeshRenderer _browRenderer;
		[SerializeField] private Material _hairMaterial;

		private NetworkManager _networkManager;
		private UserManager _userManager;

		#endregion

		#region lifecycle

		public override void OnStartClient()
		{
			base.OnStartClient();

			_networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
			_userManager = GameObject.FindGameObjectWithTag("UserManager").GetComponent<UserManager>();
			
			UserInfoUpdated(Owner.ClientId);
			_userManager.OnUserInfoUpdated += UserInfoUpdated;

			if (IsOwner) return;
			enabled = false;
			/*
			_username.gameObject.layer = LayerMask.NameToLayer("RemoteAvatar");
			_avatar.layer = LayerMask.NameToLayer("RemoteAvatar");
			_leftHandVisual.layer = LayerMask.NameToLayer("RemoteAvatar");
			_rightHandVisual.layer = LayerMask.NameToLayer("RemoteAvatar");
			*/
			
			var children = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
			foreach (var child in children)
			{
				child.gameObject.layer = LayerMask.NameToLayer("RemoteAvatar");
			}
		}

		public override void OnStopClient()
		{	
			base.OnStopClient();

			_userManager.OnUserInfoUpdated -= UserInfoUpdated;
		}

		private void Update()
		{
			if (_cameraVariable.TryGet(out var cam))
			{
				var camTransform = cam.transform;
				_head.SetPositionAndRotation(camTransform.position, camTransform.rotation);
			}

			if (_leftHandTransformVariable.TryGet(out var leftHand))
			{
				_leftHand.SetPositionAndRotation(
					leftHand.position,
					leftHand.rotation
				);
			}

			if (_rightHandTransformVariable.TryGet(out var rightHand))
			{
				_rightHand.SetPositionAndRotation(
					rightHand.position,
					rightHand.rotation
				);
			}

			_canvas.transform.position = _head.position + _usernameOffset;
		}

		#endregion

		#region private methods

		private void UserInfoUpdated(int clientID)
		{
			if (clientID != Owner.ClientId) return;

			if (!_userManager.TryGetUserInfo(Owner, out var userInfo))
				throw new("The Owner for this object does not exist!");

			_username.text = userInfo.Name;
			transform.name = IsOwner ? "LocalClient" : $"RemoteClient#{userInfo.Name}";

			_shirtRenderer.material = Instantiate(_shirtMaterial);
			_shirtRenderer.material.color = userInfo.Color;

			_hairRenderer.sharedMesh = _networkManager.GetHairMesh(userInfo.HairMesh);
			var hairMaterial = Instantiate(_hairMaterial);
			hairMaterial.color = userInfo.HairColor;
			_hairRenderer.material = _browRenderer.material = hairMaterial;
		}

		#endregion
	}
}
