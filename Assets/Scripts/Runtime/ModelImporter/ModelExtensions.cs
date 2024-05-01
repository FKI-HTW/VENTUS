using FishNet.Serializing;
using UnityEngine;

namespace VENTUS.ModelImporter
{
    public static class ModelExtensions
    {
	    public static Bounds TransformBounds(this Bounds bounds, Matrix4x4 transformation)
	    {
		    var boundsMin = bounds.min;
		    var boundsMax = bounds.max;
		    var positions = new[]
		    {
			    boundsMin, 
			    boundsMax,
			    new Vector3(boundsMin.x, boundsMin.y, boundsMax.z),
			    new Vector3(boundsMin.x, boundsMax.y, boundsMin.z),
			    new Vector3(boundsMax.x, boundsMin.y, boundsMin.z),
			    new Vector3(boundsMin.x, boundsMax.y, boundsMax.z),
			    new Vector3(boundsMax.x, boundsMin.y, boundsMax.z),
			    new Vector3(boundsMax.x, boundsMax.y, boundsMin.z),
		    };
		    return GeometryUtility.CalculateBounds(positions, transformation);
	    }

	    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
	    {
		    return new(
			    matrix.m03,
			    matrix.m13,
			    matrix.m23
			);
	    }
	    
	    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
	    {
		    return matrix.rotation;
	    }

	    public static Vector3 ExtractScale(this Matrix4x4 matrix)
	    {
		    Vector3 scale;
		    scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
		    scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
		    scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;

		    return scale;
	    }
	    
	    public static void FromMatrix(this Transform transform, Matrix4x4 matrix)
	    {
		    transform.localScale = matrix.ExtractScale();
		    transform.localRotation = matrix.ExtractRotation();
		    transform.localPosition = matrix.ExtractPosition();
	    }
	    
        public static void WriteTexture2D(this Writer writer, Texture2D texture)
		{
			if (texture == null)
			{
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				writer.WriteByte(0);
				writer.WriteInt32(0);
				return;
			}
			
			var textureData = texture.GetRawTextureData();
			writer.WriteInt32(texture.width);
			writer.WriteInt32(texture.height);
			writer.WriteByte((byte)texture.format);
			writer.WriteInt32(textureData.Length);
			writer.WriteArraySegment(textureData);
		}

        public static Texture2D ReadTexture2D(this Reader reader)
		{
			int width = reader.ReadInt32();
			int height = reader.ReadInt32();
			byte format = reader.ReadByte();
			int dataLength = reader.ReadInt32();
			if (width == 0 && height == 0 && dataLength == 0)
				return null;
			
			byte[] textureData = reader.ReadArraySegment(dataLength).ToArray();

			Texture2D texture = new(width, height, (TextureFormat)format, false);
			texture.LoadRawTextureData(textureData);
			texture.Apply();
			return texture;
		}

        public static void WriteMesh(this Writer writer, Mesh mesh)
		{
			if (mesh == null)
			{
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				return;
			}
			
			writer.WriteInt32(mesh.vertices.Length);
			foreach (Vector3 vertex in mesh.vertices)
				writer.WriteVector3(vertex);

			writer.WriteInt32(mesh.triangles.Length);
			foreach (int triangle in mesh.triangles)
				writer.WriteInt32(triangle);

			writer.WriteInt32(mesh.uv.Length);
			foreach (Vector2 uv in mesh.uv)
				writer.WriteVector2(uv);
		}

        public static Mesh ReadMesh(this Reader reader)
		{
			int verticesLength = reader.ReadInt32();
			Vector3[] vertices = new Vector3[verticesLength];
			for (int i = 0; i < verticesLength; i++)
				vertices[i] = reader.ReadVector3();

			int trianglesLength = reader.ReadInt32();
			int[] triangles = new int[trianglesLength];
			for (int i = 0; i < trianglesLength; i++)
				triangles[i] = reader.ReadInt32();

			int uvLength = reader.ReadInt32();
			Vector2[] uv = new Vector2[uvLength];
			for (int i = 0; i < uvLength; i++)
				uv[i] = reader.ReadVector2();

			if (verticesLength == 0 && trianglesLength == 0 && uvLength == 0)
				return null;
			
			Mesh mesh = new()
			{
				vertices = vertices,
				triangles = triangles,
				uv = uv
			};

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			return mesh;
		}

		public static void WriteModelObjectData(this Writer writer, ModelObjectData modelObjectData)
		{
			writer.WriteByte((byte)modelObjectData.ModelType);
			writer.WriteString(modelObjectData.Name);
			writer.WriteMatrix4x4(modelObjectData.Transformation);
			writer.WriteVector3(modelObjectData.Bounds.center);
			writer.WriteVector3(modelObjectData.Bounds.size);
			writer.WriteMesh(modelObjectData.Mesh);
			writer.WriteColor(modelObjectData.Color);
			writer.WriteTexture2D(modelObjectData.Texture);
			writer.WriteInt32(modelObjectData.Children.Count);
			foreach (var child in modelObjectData.Children)
				WriteModelObjectData(writer, child);
		}

		public static ModelObjectData ReadModelObjectData(this Reader reader)
		{
			ModelObjectData modelObjectData = new()
			{
				ModelType = (EModelType)reader.ReadByte(),
				Name = reader.ReadString(),
				Transformation = reader.ReadMatrix4x4(),
				Bounds = new(reader.ReadVector3(), reader.ReadVector3()),
				Mesh = reader.ReadMesh(),
				Color = reader.ReadColor(),
				Texture = reader.ReadTexture2D()
			};
			
			var childrenCount = reader.ReadInt32();
			for (var i = 0; i < childrenCount; i++)
				modelObjectData.Children.Add(ReadModelObjectData(reader));

			return modelObjectData;
		}
		
		public static void WriteSingleModelObjectData(this Writer writer, SingleModelObjectData singleModelObjectData)
		{
			writer.WriteByte((byte)singleModelObjectData.ModelType);
			writer.WriteString(singleModelObjectData.Name);
			writer.WriteMatrix4x4(singleModelObjectData.Transformation);
			writer.WriteVector3(singleModelObjectData.Bounds.center);
			writer.WriteVector3(singleModelObjectData.Bounds.size);
			writer.WriteMesh(singleModelObjectData.Mesh);
			writer.WriteColor(singleModelObjectData.Color);
			writer.WriteTexture2D(singleModelObjectData.Texture);
		}

		public static SingleModelObjectData ReadSingleModelObjectData(this Reader reader)
		{
			SingleModelObjectData singleModelObjectData = new()
			{
				ModelType = (EModelType)reader.ReadByte(),
				Name = reader.ReadString(),
				Transformation = reader.ReadMatrix4x4(),
				Bounds = new(reader.ReadVector3(), reader.ReadVector3()),
				Mesh = reader.ReadMesh(),
				Color = reader.ReadColor(),
				Texture = reader.ReadTexture2D()
			};

			return singleModelObjectData;
		}
	}
}
