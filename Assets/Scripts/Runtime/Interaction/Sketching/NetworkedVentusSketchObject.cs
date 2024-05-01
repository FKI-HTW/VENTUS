using System;
using System.Collections.Generic;
using VRSketchingGeometry.Commands;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

namespace VENTUS.Interaction.Sketching
{
	[RequireComponent(typeof(VentusSketchObject))]
	public class NetworkedVentusSketchObject : NetworkBehaviour
	{
		#region fields

		[SerializeField] private VentusSketchObject _sketchObject;
		[SerializeField] private MeshRenderer _renderer;
		[SerializeField] private MeshRenderer _childRenderer;
		
		[SerializeField] private Material _material;

		[SyncVar(Channel = Channel.Reliable, OnChange = nameof(SetLocalLineDiameter))]
		private float _lineDiameter = 0.0159f; // 10% of range between 0.001 - 0.15

		[SyncVar(Channel = Channel.Reliable, OnChange = nameof(SetLocalColor))]
		private Color _currentColor = Color.white;

		private static readonly CommandInvoker Invoker = new();

		// Actions buffered by late joining client while importing server control points
		private readonly List<Action> _bufferedActions = new();
		private readonly List<Vector3> _serverWorldSplinePositions = new();
		private bool _isImportingControlPoints;

		#endregion

		#region lifecycle

		public override void OnStartClient()
		{
			base.OnStartClient();

			if (_sketchObject != null)
				_sketchObject = GetComponent<VentusSketchObject>();
			if (_renderer != null)
				_renderer = GetComponent<MeshRenderer>();
			_renderer.material = Instantiate(_material);
			if (_childRenderer != null)
				_childRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
			_childRenderer.material = Instantiate(_material);

			if (!InstanceFinder.IsServer)
			{
				_isImportingControlPoints = true;
				RequestControlPoints();
			}
		}

		private void SetLocalLineDiameter(float prev, float next, bool asServer)
		{
			_sketchObject.SetLineDiameter(next);
			transform.GetChild(0).localScale = Vector3.one * next;
		}

		private void SetLocalColor(Color prev, Color next, bool asServer)
		{
			_renderer.material.color = next;
			_renderer.material.SetColor("_Color", next);
			_childRenderer.material.color = next;
			_childRenderer.material.SetColor("_Color", next);
		}

		#endregion

		#region sketching commands

		[ServerRpc(RequireOwnership = false)]
		public void SetLineDiameter(float lineDiameter, NetworkConnection conn = null)
		{
			_lineDiameter = lineDiameter;
		}

		[ServerRpc(RequireOwnership = false)]
		public void SetColor(Color color, NetworkConnection conn = null)
		{
			_currentColor = color;
		}

		[ServerRpc]
		public void AddControlPoint(Vector3 position, NetworkConnection conn = null)
		{
			_serverWorldSplinePositions.Add(position);
			AddControlPoint(position);
		}

		[ObserversRpc]
		private void AddControlPoint(Vector3 position)
		{
			if (_isImportingControlPoints)
				_bufferedActions.Add(() => _sketchObject.AddControlPointContinuous(position));
			else
				_sketchObject.AddControlPointContinuous(position);
		}

		private void RemoveControlPoint() { }

		#endregion

		#region update new clients

		[ServerRpc(RequireOwnership = false)]
		private void RequestControlPoints(NetworkConnection conn = null)
		{
			UpdateNewClient(conn, _serverWorldSplinePositions);
		}

		[TargetRpc]
		private void UpdateNewClient(NetworkConnection conn, List<Vector3> controlPoints)
		{
			// apply control points buffered by server before finalising spawn
			foreach (Vector3 controlPoint in controlPoints)
				_sketchObject.AddControlPointContinuous(controlPoint);

			// apply actions buffered by client while requesting server control points
			foreach (Action action in _bufferedActions)
				action.Invoke();
			_isImportingControlPoints = false;
		}

		#endregion
	}
}
