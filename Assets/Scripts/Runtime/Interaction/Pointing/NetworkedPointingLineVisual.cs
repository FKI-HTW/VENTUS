using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.Core.Interaction;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Networking;

namespace VENTUS.Interaction.Pointing
{
    public class NetworkedPointingLineVisual : NetworkBehaviour, IInteractionActivatable
    {
        #region fields
        
        [Header("Line")]
        [SerializeField] private int _positionCount;
        [SerializeField] private float _distance;
        
        [Header("Reticle")]
        [SerializeField] private GameObject _reticle;
        [SerializeField] private float _scalingFactor = 1;
        
        [Header("Controller")]
        [SerializeField] private bool _isLeft = true;
        
        private InteractorGroup _currentInteractorGroup;
        private Color _userColor;
        private GameObject _instantiatedReticle;
        private int _entryLayer;
        
        #endregion

        #region public methods

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            gameObject.SetActive(false);
            
            if (!IsOwner)
                return;
            
            InteractionTracking.AddElement(InteractionTypes.Pointing, this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            InteractionTracking.RemoveElement(InteractionTypes.Pointing, this);
        }
        
        public void OnEnableInteraction()
        {
            InteractionController.InteractorDown += EnableLine;
            InteractionController.InteractorUp += DisableLine;
        }

        public void OnDisableInteraction()
        {
            InteractionController.InteractorDown -= EnableLine;
            InteractionController.InteractorUp -= DisableLine;
            
            if (_currentInteractorGroup == null)
                return;
            
            DisableLine(_currentInteractorGroup);
        }

        #endregion
        
        private void Update()
        {
            if (!TryGetComponent(out LineRenderer lineRenderer)) return;

            lineRenderer.enabled = Controlling.SceneManager.IsInXRMode;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = transform.position + transform.forward * _distance;
            
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                endPosition = hit.point;
                
                //update line renderer
                if (hit.transform.GetComponent<NetworkInteractionSelectable>() != null)
                {
                    UpdateGradientColors(lineRenderer, _userColor);
                }
                else
                {
                    UpdateGradientColors(lineRenderer, Color.white);
                }

                //setup reticle
                if (_instantiatedReticle == null)
                {
                    _instantiatedReticle = Instantiate(_reticle, transform);
                }
                
                //update position
                _instantiatedReticle.transform.position = hit.point;
                
                //update rotation
                var vectorToProject = transform.up;
                
                // If surface normal is directly up indicating a horizontal surface, align the reticle's Z axis with
                // the direction of the interactor's raycast direction. Multiple by dot product to flip reticle when
                // on the underside of a horizontal surface.
                
                var targetNormalProjectedVectorDotProduct = Vector3.Dot(hit.normal, vectorToProject);
                if (Mathf.Approximately(Mathf.Abs(targetNormalProjectedVectorDotProduct), 1f)) 
                    vectorToProject = transform.forward * targetNormalProjectedVectorDotProduct;
                
                // Calculate the projected forward vector on the target normal
                var forwardVector = Vector3.ProjectOnPlane(vectorToProject, hit.normal);
                if(forwardVector != Vector3.zero)
                    _instantiatedReticle.transform.rotation = Quaternion.LookRotation(forwardVector, hit.normal);
                
                //update scale
                var scaleFactor = _scalingFactor;
                scaleFactor *= Vector3.Distance(transform.transform.position, endPosition);
                _instantiatedReticle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }
            else
            {
                //update line renderer
                UpdateGradientColors(lineRenderer, Color.white);
                
                //finalize reticle
                if (_instantiatedReticle != null)
                {
                    Destroy(_instantiatedReticle);
                    _instantiatedReticle = null;
                }
            }

            //set lineRenderer positions
            Vector3[] positions = new Vector3[_positionCount];
            Vector3 step = (endPosition - startPosition) / _positionCount;
            for (var index = 0; index < positions.Length; index++)
            {
                positions[index] = startPosition + (step * index);
            }
            
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

        #region private methods
        
        private void EnableLine(InteractorGroup interactorGroup)
        {
            if (_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Left)
                return;

            if (!_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Right)
                return;

            _currentInteractorGroup = interactorGroup;
            
            gameObject.SetActive(true);
            
            _entryLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("FrontRendering");
            
            EnableXRInteractorLineVisualServer(true, InstanceFinder.ClientManager.Connection);
        }

        private void DisableLine(InteractorGroup interactorGroup)
        {
            if (_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Left)
                return;

            if (!_isLeft && interactorGroup.EnabledInteractionInstances.First().SelectionFlags != SingleSelectorFlags.Right)
                return;
            
            _currentInteractorGroup = null;
            
            gameObject.SetActive(false);

            if (gameObject.layer == LayerMask.NameToLayer("FrontRendering"))
            {
                gameObject.layer = _entryLayer;
            }
            
            EnableXRInteractorLineVisualServer(false, InstanceFinder.ClientManager.Connection);
        }

        [ServerRpc]
        private void EnableXRInteractorLineVisualServer(bool setEnabled, NetworkConnection networkConnection)
        {
            EnableXRInteractorLineVisual(setEnabled, networkConnection);
        }
    
        [ObserversRpc(RunLocally = true)]
        private void EnableXRInteractorLineVisual(bool setEnabled, NetworkConnection networkConnection)
        {
            UserManager userManager = GameObject.FindWithTag("UserManager").GetComponent<UserManager>();
            
            _userColor = userManager.TryGetUserInfo(networkConnection, out UserInfo userInfo) ? userInfo.Color : Color.white;
                
            gameObject.SetActive(setEnabled);
        }
        
        private void UpdateGradientColors(LineRenderer lineRenderer, Color color)
        {
            var startColor = lineRenderer.startColor;
            var endColor = lineRenderer.endColor;
            
            lineRenderer.startColor = new Color(color.r, color.g, color.b, startColor.a);
            lineRenderer.endColor = new Color(color.r, color.g, color.b, endColor.a);
        }
        
        #endregion
    }
}
