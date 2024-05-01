using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public class InformationView : MonoBehaviour
    {
        #region Fields
            
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _confirmButton;
            
        private string _message;
        private Action _onConfirmAction;
        private bool _isAcceptAdded;
            
        #endregion
        
        #region Unity Lifecycle

        private void Start()
        {
            RegisterButtonEvents();
        }

        private void OnDestroy()
        {
            UnregisterButtonEvents();
        }

        #endregion
        
        #region Private Methods
    
        private void AcceptRequest()
        {
            _onConfirmAction?.Invoke();
        }

        private void RegisterButtonEvents()
        {
            if (_confirmButton)
            {
                _isAcceptAdded = true;
                _confirmButton.onClick.AddListener(AcceptRequest);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (_isAcceptAdded)
            {
                _isAcceptAdded = false;
                _confirmButton.onClick.RemoveListener(AcceptRequest);
            }
        }
            
        #endregion
            
        #region Public Methods
            
        public void AddInformationContext(string message, Action onConfirmAction)
        {
            _messageText.text = message;
            _onConfirmAction = onConfirmAction;
        }
        
        #endregion
    }
}
