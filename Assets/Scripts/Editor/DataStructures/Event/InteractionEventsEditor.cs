using UnityEditor;
using UnityEngine;

namespace VENTUS.DataStructures.Event
{
    [CustomEditor(typeof(ActionEvent), editorForChildClasses: true)]
    public class InterinteractionEventsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            ActionEvent e = target as ActionEvent;
            if (GUILayout.Button("Raise"))
            {
                e.Raise();
            }
        }
    }
}
