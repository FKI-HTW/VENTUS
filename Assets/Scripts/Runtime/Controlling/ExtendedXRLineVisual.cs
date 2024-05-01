using System;
using FishNet;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Networking;

namespace VENTUS.Controlling
{
    public class ExtendedXRLineVisual : MonoBehaviour
    {
        [SerializeField] private XRInteractorLineVisual _xrInteractorLineVisual;

        private GradientColorKey[] _entryColorKeys;
        private GradientColorKey[] _userColorKeys;
        private Color _userColor;
        private UserManager _userManager;

        private void Start()
        {
            _entryColorKeys = _xrInteractorLineVisual.invalidColorGradient.colorKeys;
            _userManager = GameObject.FindWithTag("UserManager").GetComponent<UserManager>();
        }

        void Update()
        {
            if (!_userManager.TryGetUserInfo(InstanceFinder.ClientManager.Connection, out UserInfo userInfo))
                return;

            if (_userColor != userInfo.Color)
            {
                _userColor = userInfo.Color;
                
                _userColorKeys = new GradientColorKey[_xrInteractorLineVisual.invalidColorGradient.colorKeys.Length];
                for (var index = 0; index < _userColorKeys.Length; index++)
                {
                    _userColorKeys[index] = new GradientColorKey(userInfo.Color, _xrInteractorLineVisual.invalidColorGradient.colorKeys[index].time);
                }
                
                GradientColorKey[] validColorKeys = new GradientColorKey[_xrInteractorLineVisual.validColorGradient.colorKeys.Length];
                for (var index = 0; index < validColorKeys.Length; index++)
                {
                    validColorKeys[index] = new GradientColorKey(userInfo.Color, _xrInteractorLineVisual.validColorGradient.colorKeys[index].time);
                }
                _xrInteractorLineVisual.validColorGradient.colorKeys = validColorKeys;
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit) && 
                raycastHit.transform.TryGetComponent(out ISelectable _))
            {
                
                _xrInteractorLineVisual.invalidColorGradient.colorKeys = _userColorKeys;
            }
            else
            {
                _xrInteractorLineVisual.invalidColorGradient.colorKeys = _entryColorKeys;
            }
        }
    }
}
