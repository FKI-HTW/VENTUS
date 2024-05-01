using GLTFast;
using UnityEngine;
using Unity.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VENTUS.ModelImporter
{
    public static class GLTFImporter
    {
        public static async Task<ModelObjectData> ParseFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("The given path is invalid!");
                return null;
            }

            var gltf = new GltfImport();
            var settings = new ImportSettings()
            {
                GenerateMipMaps = true
            };
            var success = await gltf.Load($"file://{path}", settings);
            if (!success)
            {
                Debug.Log("The given file could not be loaded!");
                return null;
            }

            var instantiator = new ModelObjectDataInstantiator(gltf, Path.GetFileName(path));
            await gltf.InstantiateMainSceneAsync(instantiator);
            return instantiator.ModelObjectData;
        }
        
        private class ModelObjectDataInstantiator : IInstantiator
        {
            public ModelObjectData ModelObjectData { get; private set; }

            private readonly GltfImport _gltf;
            private readonly string _fileName;
            private readonly Dictionary<uint, ModelObjectData> _nodes = new();

            public ModelObjectDataInstantiator(GltfImport gltf, string fileName)
            {
                _gltf = gltf;
                _fileName = fileName;
            }
            
            public void BeginScene(string name, uint[] rootNodeIndices)
            {
                ModelObjectData = new()
                {
                    ModelType = EModelType.ModelParent,
                    Name = _fileName,
                    Transformation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
                    Bounds = new(),
                    Mesh = null,
                    Color = Color.white,
                    Texture = null
                };
            }

            public void CreateNode(uint nodeIndex, uint? parentIndex, Vector3 position, Quaternion rotation, Vector3 scale)
            {
                ModelObjectData modelObjectData = new()
                {
                    ModelType = EModelType.ModelPart,
                    Name = "GLTFNode",
                    Transformation = Matrix4x4.TRS(position, rotation, scale),
                    Bounds = new(),
                    Mesh = null,
                    Color = Color.white,
                    Texture = null
                };

                if (parentIndex.HasValue)
                {
                    var parent = _nodes[parentIndex.Value];
                    parent.ModelType = EModelType.ModelProduct;
                    parent.Children.Add(modelObjectData);
                }
                else
                {
                    ModelObjectData.Children.Add(modelObjectData);
                }
                
                _nodes.Add(nodeIndex, modelObjectData);
            }

            public void SetNodeName(uint nodeIndex, string name)
            {
                _nodes[nodeIndex].Name = name;
            }

            public void AddPrimitive(uint nodeIndex, string meshName, MeshResult meshResult, uint[] joints = null, uint? rootJoint = null,
                float[] morphTargetWeights = null, int primitiveNumeration = 0)
            {
                ModelObjectData meshGo;
                if (primitiveNumeration == 0)
                {
                    meshGo = _nodes[nodeIndex];
                }
                else
                {
                    var parentMesh = _nodes[nodeIndex];
                    parentMesh.ModelType = EModelType.ModelProduct;

                    meshGo = new()
                    {
                        ModelType = EModelType.ModelPart,
                        Name = $"{parentMesh.Name}.Mesh{primitiveNumeration}",
                        Transformation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
                        Bounds = new(),
                        Mesh = null,
                        Color = Color.white,
                        Texture = null
                    };
                        
                    parentMesh.Children.Add(meshGo);
                }

                var hasMorphTargets = meshResult.mesh.blendShapeCount > 0;
                if (joints == null && !hasMorphTargets)
                    meshGo.Mesh = meshResult.mesh;
                else
                    Debug.LogError("Skinned meshes rendering not yet supported!");

                try
                {
                    var material = _gltf.GetMaterial(meshResult.materialIndices[0]) ?? _gltf.GetDefaultMaterial();
                    
                    meshGo.Color = material.color;
                    // meshGo.Texture = material.mainTexture as Texture2D;
                } catch (NullReferenceException) {}
            }

            public void AddPrimitiveInstanced(uint nodeIndex, string meshName, MeshResult meshResult, uint instanceCount,
                NativeArray<Vector3>? positions, NativeArray<Quaternion>? rotations, NativeArray<Vector3>? scales, int primitiveNumeration = 0)
            {
                Debug.LogError("Instanced primitive rendering not yet supported!");
            }

            public void AddAnimation(AnimationClip[] animationClips) {}
            public void AddCamera(uint nodeIndex, uint cameraIndex) {}
            public void AddLightPunctual(uint nodeIndex, uint lightIndex) {}
            public void EndScene(uint[] rootNodeIndices) {}
        }
    }
}
