using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public class ConfirmView : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;
        
        private string _message;
        private Action _onAcceptAction;
        private bool _isAcceptAdded;
        private Action _onDeclineAction;
        private bool _isDeclineAdded;
        
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
            _onAcceptAction?.Invoke();
        }

        private void DeclineRequest()
        {
            _onDeclineAction?.Invoke();
        }

        private void RegisterButtonEvents()
        {
            if (_acceptButton)
            {
                _isAcceptAdded = true;
                _acceptButton.onClick.AddListener(AcceptRequest);
            }

            if (_declineButton)
            {
                _isDeclineAdded = true;
                _declineButton.onClick.AddListener(DeclineRequest);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (_isAcceptAdded)
            {
                _isAcceptAdded = false;
                _acceptButton.onClick.RemoveListener(AcceptRequest);
            }

            if (_isDeclineAdded)
            {
                _isDeclineAdded = false;
                _declineButton.onClick.RemoveListener(DeclineRequest);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        public void AddConfirmContext(string message, Action onAcceptAction = null, Action onDeclineAction = null)
        {
            _messageText.text = message;
            _onAcceptAction = onAcceptAction;
            _onDeclineAction = onDeclineAction;
        }
        
        #endregion
    }
}
