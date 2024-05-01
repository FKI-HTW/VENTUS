using System;
using Plugins.QuickOutline.Scripts;
using UnityEngine;

namespace VENTUS.Networking
{
    public class NetworkOutlineByOwnership : OwnershipObserver
    {
        [SerializeField] private UserManager _userManager;
        [SerializeField] private Outline.Mode _outlineMode;
        [SerializeField] private float _outlineWidth;
        
        private Outline _outline;
        private bool _initOutline;
        
        private void Awake()
        {
            if (_userManager == null)
                _userManager = GameObject.FindWithTag("UserManager").GetComponent<UserManager>();
        }
        
        protected override void OnLockOwnershipClient()
        {
            base.OnLockOwnershipClient();

            _initOutline = true;
        }

        protected override void OnUnlockOwnershipClient()
        {
            base.OnUnlockOwnershipClient();

            DestroyOutline();
        }

        private void Update()
        {
            if (_initOutline && _userManager.TryGetUserInfo(Owner, out UserInfo userInfo))
            {
                InstantiateOutline(userInfo.Color);
                _initOutline = false;
            }
        }

        private void InstantiateOutline(Color color)
        {
            if (!_outline)
            {
                _outline = gameObject.AddComponent<Outline>();
                _outline.UpdateOnMeshChanged = true;
            }

            _outline.OutlineColor = color;
            _outline.OutlineMode = _outlineMode;
            _outline.OutlineWidth = _outlineWidth;
        }

        private void DestroyOutline()
        {
            if (_outline)
            {
                Destroy(_outline);
            }
        }
    }
}
