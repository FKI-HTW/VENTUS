using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.DataStructures.CommandLogic.Disposable;
using VENTUS.DataStructures.CommandLogic.Snapshot;
using VENTUS.DataStructures.Variables;
using System;
using System.Collections.Generic;
using System.IO;

namespace VENTUS.ModelImporter
{
	public class ModelManager : NetworkBehaviour
	{
		[SerializeField] private CameraVariable _cameraVariable;
		[SerializeField] private float _distance = 5f;

		[Header("Library Model References")] 
		[SerializeField] private LibraryModels _libraryModels;
		[SerializeField] private GameObject _modelPreviewPrefab;
		[SerializeField] private Material	_modelPreviewMaterial;
		[SerializeField] private Button		_modelButtonPrefab;

		[Header("Networked Model References")]
		[SerializeField] private Transform _modelContainer;
		[SerializeField] private NetworkedModelObject _modelParentPrefab;
		[SerializeField] private NetworkedModelObject _modelProductPrefab;
		[SerializeField] private NetworkedModelObject _modelPartPrefab;

		public void LoadLibraryModels(Transform parent)
		{
			if (_libraryModels == null)
				return;
			
			foreach (var libraryModel in _libraryModels.LibraryModelObjects)
			{
				Button btn = Instantiate(_modelButtonPrefab, parent);
				CreateLibraryModelPreview(libraryModel.ParentID, btn.transform.GetChild(0), true);
				btn.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = libraryModel.Name;
				btn.onClick.AddListener(delegate { ImportLibraryModel(libraryModel); });
				continue;

				void CreateLibraryModelPreview(int id, Transform modelParent, bool isRoot)
				{
					if (!libraryModel.ObjectData.TryGetValue(id, out var singleModelObjectData))
						throw new NullReferenceException($"The Model Object Data with the ID {id} does not exist!");

					var previewObject = Instantiate(_modelPreviewPrefab, modelParent, true);
			
					previewObject.GetComponent<MeshFilter>().mesh = singleModelObjectData.Mesh;
					var previewMat = Instantiate(_modelPreviewMaterial);
					if (singleModelObjectData.Texture != null)
						previewMat.mainTexture = singleModelObjectData.Texture;
					previewMat.color = singleModelObjectData.Color;
					previewObject.GetComponent<MeshRenderer>().material = previewMat;

					if (isRoot)
					{
						previewObject.transform.localPosition = libraryModel.Position;
						previewObject.transform.localEulerAngles = libraryModel.Rotation;
						previewObject.transform.localScale = libraryModel.Scale;
					}
					else
					{
						previewObject.transform.FromMatrix(singleModelObjectData.Transformation);
					}

					if (!libraryModel.Connections.TryGetValue(id, out var childIDs))
						return;
			
					foreach (var childID in childIDs.IDs)
						CreateLibraryModelPreview(childID, previewObject.transform, false);
				}
			}
		}
		
		public void ImportLibraryModel(LibraryModel libraryModel)
		{
			if (!_cameraVariable.TryGet(out var cam))
			{
				Debug.Log("The camera has to be assigned to import a model!");
				return;
			}
			
			var camTransform = cam.transform;
			var targetPosition = camTransform.position + camTransform.forward * _distance;

			var modelObjectData = CreateModelObjectData(libraryModel.ParentID);

			SpawnModel(modelObjectData, targetPosition);
			return;

			ModelObjectData CreateModelObjectData(int id)
			{
				if (!libraryModel.ObjectData.TryGetValue(id, out var singleModelObjectData))
					throw new NullReferenceException($"The Model Object Data with the ID {libraryModel.ParentID} does not exist!");

				ModelObjectData newModelObjectData = new(singleModelObjectData);
				if (!libraryModel.Connections.TryGetValue(id, out var childIDs))
					return newModelObjectData;
			
				foreach (var childID in childIDs.IDs)
					newModelObjectData.Children.Add(CreateModelObjectData(childID));

				return newModelObjectData;
			}
		}
		
		public async void ImportModel(string path)
		{
			if (!_cameraVariable.TryGet(out var cam))
			{
				Debug.Log("The camera has to be assigned to import a model!");
				return;
			}

			var camTransform = cam.transform;
			var targetPosition = camTransform.position + camTransform.forward * _distance;
			
			var ext = Path.GetExtension(path);
			ModelObjectData modelObjectData;
			switch (ext)
			{
				case ".gltf":
				case ".glb":
					modelObjectData = await GLTFImporter.ParseFile(path);
					break;
				case ".step":
				case ".stp":
					modelObjectData = STEPImporter.ParseFile(path);
					break;
				default:
					Debug.LogError("The given file has an unsupported type!");
					return;
			}

			if (modelObjectData == null)
			{
				Debug.LogError("Something went wrong while creating the model!");
				return;
			}
			
			SpawnModel(modelObjectData, targetPosition);
		}


		[ServerRpc(RequireOwnership = false)]
		private void SpawnModel(ModelObjectData modelObjectData, Vector3 targetPosition, 
			Channel channel = Channel.Reliable, NetworkConnection conn = null)
		{
			// spawn parent
			var modelObject = Instantiate(_modelParentPrefab, _modelContainer);
			Spawn(modelObject.gameObject, LocalConnection);

			//initialize and add it to the redo/undo system
			var previousCommandSnapshot = new List<ICommandSnapshot>();
			var currentCommandSnapshot = new List<ICommandSnapshot>();
			var relatedObject = new List<GameObject>();
			PushToCommandSnapshots(modelObject.gameObject, relatedObject, previousCommandSnapshot, currentCommandSnapshot);
			
			// spawn children
			foreach (var child in modelObjectData.Children)
				SpawnSubModel(conn, child, modelObject.transform, modelObject, 
					relatedObject, previousCommandSnapshot, currentCommandSnapshot);
			
			// calculate parent bounding box
			Bounds bounds = new();
			var renderers = modelObject.gameObject.GetComponentsInChildren<MeshRenderer>();
			if (renderers.Length > 0)
			{
				bounds = renderers[0].bounds;
				foreach (var renderer in renderers)
					bounds.Encapsulate(renderer.bounds);
			}

			// scale the object transformation and bounds
			bounds = bounds.TransformBounds(modelObjectData.Transformation);
			modelObject.transform.SetPositionAndRotation(
				new(targetPosition.x, -bounds.min.y, targetPosition.z),
				modelObjectData.Transformation.rotation
			);

			modelObject.Initialise(new(modelObjectData), null);

			ICommandDisposable commandDisposable = new DespawnDisposable();
			SnapshotMemento.PushToCommandHandler(
				conn, 
				relatedObject.ToArray(), 
				previousCommandSnapshot.ToArray(), 
				currentCommandSnapshot.ToArray(), 
				new [] { commandDisposable }
			);
			
			foreach (var spawnedObject in relatedObject)
				spawnedObject.GetComponent<NetworkObject>().RemoveOwnership();
		}

		private void SpawnSubModel(NetworkConnection conn, ModelObjectData modelObjectData, Transform parent, NetworkedModelObject root,
			List<GameObject> relatedObjects, List<ICommandSnapshot> previousCommandSnapshot, List<ICommandSnapshot> currentCommandSnapshot)
		{
			switch (modelObjectData.ModelType)
			{
				case EModelType.ModelPart:
					{
						// spawn object
						var modelObject = Instantiate(_modelPartPrefab, parent);
						Spawn(modelObject.gameObject, LocalConnection);
						modelObject.transform.FromMatrix(modelObjectData.Transformation);

						modelObject.Initialise(new(modelObjectData), null);
						
						PushToCommandSnapshots(modelObject.gameObject, relatedObjects, previousCommandSnapshot, currentCommandSnapshot);
						break;
					}
				case EModelType.ModelProduct:
					{
						// spawn object
						var modelObject = Instantiate(_modelProductPrefab, parent);
						Spawn(modelObject.gameObject, LocalConnection);
						
						if (modelObjectData.Mesh != null)
						{	// TODO : temporary workaround for model products that have an assigned mesh
							modelObjectData.Children.Add(new()
							{
								ModelType = EModelType.ModelPart,
								Name = $"{modelObjectData.Name}.Mesh",
								Transformation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
								Bounds = modelObjectData.Bounds,
								Mesh = modelObjectData.Mesh,
								Color = modelObjectData.Color,
								Texture = modelObjectData.Texture
							});
							modelObjectData.Bounds = default;
							modelObjectData.Mesh = null;
							modelObjectData.Color = Color.white;
							modelObjectData.Texture = new(1, 1);
						}

						modelObject.transform.FromMatrix(modelObjectData.Transformation);

						modelObject.Initialise(new(modelObjectData), null);
						
						//add it to the redo/undo system
						PushToCommandSnapshots(modelObject.gameObject, relatedObjects, previousCommandSnapshot, currentCommandSnapshot);

						foreach (var child in modelObjectData.Children)
							SpawnSubModel(conn, child, modelObject.transform, root, 
								relatedObjects, previousCommandSnapshot, currentCommandSnapshot);
						break;
					}
			}
		}
		
		private void PushToCommandSnapshots(GameObject spawnedObject, List<GameObject> relatedObjects, 
			List<ICommandSnapshot> previousCommandSnapshot, List<ICommandSnapshot> currentCommandSnapshot)
		{
			relatedObjects.Add(spawnedObject);
			previousCommandSnapshot.Add(new ActiveAndEnabledSnapshot(spawnedObject, false));
			currentCommandSnapshot.Add(new ActiveAndEnabledSnapshot(spawnedObject,  true));
		}
	}
}
