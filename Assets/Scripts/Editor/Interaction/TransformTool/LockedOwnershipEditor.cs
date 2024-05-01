using UnityEditor;
using UnityEngine;
using VENTUS.Networking;

namespace VENTUS.Interaction.TransformTool
{
    [CustomEditor(typeof(LockedOwnership))]
    public class LockedOwnershipEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LockedOwnership ownership = ((LockedOwnership)target);

            EditorGUILayout.Toggle("Can Take Ownership", ownership.CanTakeOwnership);
            EditorGUILayout.Toggle("Has Ownership", ownership.HasOwnership);

            if (GUILayout.Button("Request Ownership"))
                ownership.RequestOwnership();

            if (GUILayout.Button("Release Ownership"))
                ownership.ReleaseOwnership();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
