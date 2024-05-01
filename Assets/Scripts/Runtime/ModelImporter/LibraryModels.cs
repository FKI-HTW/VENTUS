using System;
using UnityEngine;
using VENTUS.DataStructures;

namespace VENTUS.ModelImporter
{
    public class LibraryModels : ScriptableObject
    {
        public LibraryModel[] LibraryModelObjects;
    }
    
    [Serializable]
    public class LibraryModel
    {
        public string Name;
        public int ParentID;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        public SerializableDictionary<int, ChildrenIDs> Connections;
        public SerializableDictionary<int, SingleModelObjectData> ObjectData;

        public LibraryModel(string name, int parentID, 
            SerializableDictionary<int, ChildrenIDs> connections, 
            SerializableDictionary<int, SingleModelObjectData> objectData)
        {
            Name = name;
            ParentID = parentID;
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Scale = Vector3.zero;
            Connections = connections;
            ObjectData = objectData;
        }
    }

    [Serializable]
    public class ChildrenIDs
    {
        public int[] IDs;
    }
}
