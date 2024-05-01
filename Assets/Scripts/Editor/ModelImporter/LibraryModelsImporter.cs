using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using VENTUS.DataStructures;

namespace VENTUS.ModelImporter
{
    public class LibraryModelImporter : EditorWindow
    {
		private const string MODEL_LIST_FOLDER = "LibraryModels";
		private const string MODEL_LIST_ASSET_PATH = "Assets/ScriptableObjects/ModelImporter/LibraryModels.asset";

		[MenuItem("VENTUS/Model Library Importer")]
		public static void ShowWindow()
		{
			GetWindow(typeof(LibraryModelImporter), false, "ModelLibrary Importer");
		}

		private void OnGUI()
		{
			GUILayout.Label("LibraryModel Importer", EditorStyles.largeLabel);

			if (GUILayout.Button("Import Library Models"))
				ImportLibraryModels();
		}

		private static async void ImportLibraryModels()
		{
			var modelListUrl = Path.Combine(Application.streamingAssetsPath, $"{MODEL_LIST_FOLDER}/ModelList.json");
			StreamReader reader = new(modelListUrl);
			var json = await reader.ReadToEndAsync();
			reader.Close();
			reader.Dispose();
			
			var exampleModels = JsonConvert.DeserializeObject<string[]>(json);

			var libraryModels = CreateInstance<LibraryModels>();
			AssetDatabase.CreateAsset(libraryModels, MODEL_LIST_ASSET_PATH);
			libraryModels.LibraryModelObjects = new LibraryModel[exampleModels.Length];

			for (var i = 0; i < exampleModels.Length; i++)
			{
				var path = Path.Combine(Application.streamingAssetsPath, MODEL_LIST_FOLDER, exampleModels[i]);
				var ext = Path.GetExtension(path);
				var fileName = Path.GetFileName(path);
				
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
						continue;
				}

				var id = 0;
				SerializableDictionary<int, ChildrenIDs> connections = new();
				SerializableDictionary<int, SingleModelObjectData> flattenedModelData = new();
				ImportFlattenedLibraryModels(modelObjectData);
				
				libraryModels.LibraryModelObjects[i] = new(fileName, 0, connections, flattenedModelData);
				continue;

				void ImportFlattenedLibraryModels(ModelObjectData modelData)
				{
					var currentID = id;
					
					var childIds = new int[modelData.Children.Count];
					
					SingleModelObjectData singleModelData = new(modelData);
					if (singleModelData.Mesh != null)
						AssetDatabase.AddObjectToAsset(singleModelData.Mesh, libraryModels);
					if (singleModelData.Texture != null)
						AssetDatabase.AddObjectToAsset(singleModelData.Texture, libraryModels);
					flattenedModelData.Add(currentID, singleModelData);
					
					for (var j = 0; j < modelData.Children.Count; j++)
					{
						childIds[j] = ++id;
						ImportFlattenedLibraryModels(modelData.Children[j]);
					}
					connections.Add(currentID, new() { IDs = childIds });
				}
			}

			EditorUtility.SetDirty(libraryModels);
			AssetDatabase.SaveAssets();
		}
	}
}

