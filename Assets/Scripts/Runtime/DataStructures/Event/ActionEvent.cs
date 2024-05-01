using System;
using UnityEngine;

namespace VENTUS.DataStructures.Event
{
    [CreateAssetMenu(fileName = "new ActionEvent", menuName = "VENTUS/DataStructures/Action Event")]
    public class ActionEvent : ScriptableObject
    {
        private Action listeners;
    
        public void Raise()
        {
            listeners?.Invoke();
        }

        public void RegisterListener(Action listener)
        {
            listeners += listener;
        }

        public void UnregisterListener(Action listener)
        {
            listeners -= listener;
        }
    }
}