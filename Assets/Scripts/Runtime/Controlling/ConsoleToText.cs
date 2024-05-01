using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VENTUS.Controlling
{
    public class ConsoleToText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _debugText;
        [SerializeField] private int _trackedMessagesCount;
    
        private readonly Queue<string> _previousMessages = new();
    
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            ClearLog();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (_previousMessages.Count >= _trackedMessagesCount && _previousMessages.Count > 0)
            {
                _previousMessages.Dequeue();
            }
            _previousMessages.Enqueue(DateTime.Now + " | " + logString);

            UpdateLog();
        }

        private void ClearLog()
        {
            _previousMessages.Clear();
            UpdateLog();
        }

        private void UpdateLog()
        {
            string finalText = "";
            foreach (var previousMessage in _previousMessages)
            {
                finalText = previousMessage + "\n" + finalText;
            }
            _debugText.text = finalText;
        }
    }
}
