using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Disposable;
using VENTUS.DataStructures.CommandLogic.Snapshot;
using VENTUS.DataStructures.Variables;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;
using VENTUS.Networking;

namespace VENTUS.Interaction.Sketching
{
	public class SketchingManager : NetworkBehaviour, IInteractionActivatable
	{
		[SerializeField] private NetworkedVentusSketchObject _sketchPrefab;
		[SerializeField] private Transform _sketchParent;

		[SerializeField] private TransformVariable _leftHandTransformVariable;
		[SerializeField] private TransformVariable _rightHandTransformVariable;

		[SerializeField] private float _lineDiameter = 0.0159f; // 10% of range between 0.001 - 0.15
		[SerializeField] private Color _currentColor = Color.white;

		[SerializeField] private float _controlPointOffset = 0.1f;
		[SerializeField] private float _distanceFromCameraDesktop = 1f;

		public float LineDiameter
		{
			get => _lineDiameter;
			set => _lineDiameter = value;
		}

		public Color CurrentColor
		{
			get => _currentColor;
			set => _currentColor = value;
		}

		private bool _isDrawingLeft;
		private bool _isDrawingRight;

		private readonly List<GameObject> _spawnedObjects = new();

		public override void OnStartClient()
		{
			base.OnStartClient();

			InteractionTracking.AddElement(InteractionTypes.Sketching, this);
		}

		public override void OnStopClient()
		{
			base.OnStopClient();

			InteractionTracking.RemoveElement(InteractionTypes.Sketching, this);
		}

		public void OnEnableInteraction()
		{
			InteractionController.InteractorDown += StartDraw;
			InteractionController.InteractorUp += StopDraw;
		}

		public void OnDisableInteraction()
		{
			InteractionController.InteractorDown -= StartDraw;
			InteractionController.InteractorUp -= StopDraw;

			ReleaseOwnershipAll();
		}

		private void StartDraw(InteractorGroup interactorGroup)
		{
			switch (interactorGroup.EnabledInteractionInstances.First().SelectionFlags)
			{
				case SingleSelectorFlags.Left:
					_isDrawingLeft = true;
					break;
				case SingleSelectorFlags.Right:
					_isDrawingRight = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Transform handTransform = _isDrawingLeft ? _leftHandTransformVariable.Get() : _rightHandTransformVariable.Get();
			float distance = Controlling.SceneManager.IsInXRMode ? 0f : _distanceFromCameraDesktop;
			SpawnLineSketch(_isDrawingLeft, _lineDiameter, _currentColor, handTransform.position + handTransform.forward * distance);
		}

		private void StopDraw(InteractorGroup interactorGroup)
		{
			switch (interactorGroup.EnabledInteractionInstances.First().SelectionFlags)
			{
				case SingleSelectorFlags.Left:
					_isDrawingLeft = false;
					break;
				case SingleSelectorFlags.Right:
					_isDrawingRight = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ReleaseOwnershipAll()
		{
			foreach (var spawnedObject in _spawnedObjects)
			{
				if (spawnedObject.TryGetComponent(out LockedOwnership lockedOwnership))
				{
					lockedOwnership.ReleaseOwnership();
				}
			}
			_spawnedObjects.Clear();
		}

		[ServerRpc(RequireOwnership = false)]
		private void SpawnLineSketch(bool isLeft, float lineDiameter, Color color, Vector3 originPosition, NetworkConnection conn = null)
		{
			var go = Instantiate(_sketchPrefab, originPosition, Quaternion.identity, _sketchParent);
			Spawn(go.gameObject, conn);
			go.SetLineDiameter(lineDiameter);
			go.SetColor(color);
			DrawLineSketch(conn, isLeft, go.gameObject);
		}

		[TargetRpc]
		private void DrawLineSketch(NetworkConnection conn, bool isLeft, GameObject sketchObject)
		{
			var sketch = sketchObject.GetComponent<NetworkedVentusSketchObject>();
			_spawnedObjects.Add(sketchObject);
			StartCoroutine(DrawLine());
			return;

			IEnumerator DrawLine()
			{
				var time = _controlPointOffset;
				while (isLeft && _isDrawingLeft || !isLeft && _isDrawingRight)
				{
					time += Time.deltaTime;
					if (time > _controlPointOffset)
					{
						Transform handTransform = _isDrawingLeft ? _leftHandTransformVariable.Get() : _rightHandTransformVariable.Get();
						float distance = Controlling.SceneManager.IsInXRMode ? 0f : _distanceFromCameraDesktop;
						var handPosition = handTransform.position + handTransform.forward * distance;

						sketch.AddControlPoint(handPosition);
						time = 0;
					}
					yield return null;
				}

				if (sketch.TryGetComponent(out NetworkObject networkObject))
				{
					OnStopDrawServer(networkObject);
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void OnStopDrawServer(NetworkObject sketchNetworkObject)
		{
			GameObject sketchGameObject = sketchNetworkObject.gameObject;
			ICommandSnapshot previousCommandSnapshot = new ActiveAndEnabledSnapshot(sketchGameObject, false);
			ICommandSnapshot currentCommandSnapshot = new ActiveAndEnabledSnapshot(sketchGameObject, true);
			ICommandDisposable commandDisposable = new DespawnDisposable();
			SnapshotMemento.PushToCommandHandler(sketchNetworkObject.Owner, sketchGameObject, previousCommandSnapshot, currentCommandSnapshot, commandDisposable);
		}
	}
}
