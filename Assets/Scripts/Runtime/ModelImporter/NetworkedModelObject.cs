using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace VENTUS.ModelImporter
{
    public class NetworkedModelObject : NetworkBehaviour
    {
        public EModelType ModelType;
        public NetworkedModelObject ModelRoot;
        public string Name;
        public Matrix4x4 Transformation;
        public Bounds Bounds;
        public Mesh Mesh;
        public Color Color;
        public Texture2D Texture;

        [SerializeField] private Material       _material;
        [SerializeField] private MeshFilter     _meshFilter;
        [SerializeField] private MeshRenderer   _meshRenderer;
        [SerializeField] private MeshCollider   _meshCollider;

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        public void Initialise(SingleModelObjectData modelObjectData, NetworkedModelObject root, 
            Channel channel = Channel.Reliable)
        {
            ModelRoot = root;
            ModelType = modelObjectData.ModelType;
            Name = modelObjectData.Name;
            Transformation = modelObjectData.Transformation;
            Bounds = modelObjectData.Bounds;
            Mesh = modelObjectData.Mesh;
            Color = modelObjectData.Color;
            Texture = modelObjectData.Texture;

            name = Name;
            
            _meshFilter.sharedMesh = Mesh;
            _meshCollider.enabled = false;
            _meshCollider.sharedMesh = Mesh;
            _meshCollider.enabled = true;

            var material = Instantiate(_material);
            material.color = Color;
            material.mainTexture = Texture;
            _meshRenderer.material = material;
        }
    }
}
