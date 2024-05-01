using System;
using CENTIS.UnityHierarchyView;
using FishNet.Connection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Networking;

namespace VENTUS.UI.Hierarchy
{
    public class HierarchyNode : UINode
    {
	    [SerializeField] private UserManager _userManager;
        [SerializeField] private RectTransform _indentation;
        [SerializeField] private Button _foldButton;
        [SerializeField] private Image _colorPanel;
        [SerializeField] private TMP_Text _foldButtonText;
        [SerializeField] private TMP_Text _nodeNameText;
        [SerializeField] private float _indentationMultiplier;

        private Color _initialColor;
        private OwnershipObserver _ownershipObserver;

        private void Awake()
        {
	        if (_userManager == null)
		        _userManager = GameObject.FindWithTag("UserManager").GetComponent<UserManager>();

	        _initialColor = _colorPanel.color;
        }

        private void OnDestroy()
        {
	        if (_ownershipObserver == null) 
		        return;

	        ResetColorPanel();
	        _ownershipObserver.UnregisterAction(OnLockOwnershipClient, OwnershipObserverEventType.OnLockOwnershipClient);
	        _ownershipObserver.UnregisterAction(OnUnlockOwnershipClient, OwnershipObserverEventType.OnUnlockOwnershipClient);
        }

        public override void Initiate(Transform transform, bool foldedOut, bool hasChildren, int rowIndex, int columnIndex)
		{
            gameObject.name = $"HierarchyNode: {transform.name}";
			_nodeNameText.text = transform.name;
            float indentation = columnIndex * _indentationMultiplier;
            _indentation.sizeDelta = new(_indentation.sizeDelta.x + indentation, _indentation.sizeDelta.y);
            
            if (!hasChildren)
            {
                _foldButton.interactable = false;
                _foldButtonText.gameObject.SetActive(false);
            }

            if (transform.TryGetComponent(out _ownershipObserver))
            {
	            SetColorPanelToUser();
	            //TODO: broadcast instead
	            _ownershipObserver.RegisterAction(OnLockOwnershipClient, OwnershipObserverEventType.OnLockOwnershipClient);
	            _ownershipObserver.RegisterAction(OnUnlockOwnershipClient, OwnershipObserverEventType.OnUnlockOwnershipClient);
            }
		}

		public override void OnFolded(bool fold)
		{
            _foldButtonText.text = fold ? "-" : "+";
		}
		
		private void OnLockOwnershipClient(NetworkConnection owner)
		{
			SetColorPanelToUser();
		}
		
		private void OnUnlockOwnershipClient(NetworkConnection owner)
		{
			ResetColorPanel();
		}
		
		private void SetColorPanelToUser()
		{
			if (!_userManager.TryGetUserInfo(_ownershipObserver.Owner, out UserInfo userInfo))
				return;

			_colorPanel.color = userInfo.Color;
		}

		private void ResetColorPanel()
		{
			_colorPanel.color = _initialColor;
		}
    }
}
