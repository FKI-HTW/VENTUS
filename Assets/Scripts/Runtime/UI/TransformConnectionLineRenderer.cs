using FishNet;
using UnityEngine;
using VENTUS.Networking;

namespace VENTUS.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class TransformConnectionLineRenderer : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private int _subVerticesCount;
    
        private bool _isConnected;
        private LineRenderer _lineRenderer;
        private Transform _target;
        private Transform _origin;
        
        #endregion

        #region Unity Lifecycle
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            
            UserManager userManager = GameObject.FindWithTag("UserManager").GetComponent<UserManager>();
            if (!userManager.TryGetUserInfo(InstanceFinder.ClientManager.Connection, out UserInfo userInfo))
                return;

            _lineRenderer.startColor = userInfo.Color;
            _lineRenderer.endColor = userInfo.Color;
        }
        
        private void Update()
        {
            if (!_isConnected)
                return;

            Vector3[] positions = new Vector3[_subVerticesCount + 1];
            for (int i = 0; i <= _subVerticesCount; i++)
            {
                positions[i] = Vector3.Lerp(_origin.position, _target.position, (float)i / _subVerticesCount);
            }

            _lineRenderer.positionCount = _subVerticesCount;
            _lineRenderer.SetPositions(positions);
        }
        
        #endregion

        #region Public Methods
        
        public void SetNodes(Transform origin, Transform target)
        {
            _isConnected = true;
            _origin = origin;
            _target = target;
        }
        
        public void RemoveNodes()
        {
            _isConnected = false;
            _origin = null;
            _target = null;
        }
        
        #endregion
    }
}
