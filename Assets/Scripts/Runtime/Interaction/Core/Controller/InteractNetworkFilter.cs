using UnityEngine;
using VENTUS.Networking;

namespace VENTUS.Interaction.Core.Controller
{
    public class InteractNetworkFilter : InteractFilter
    {
        [Header("Networked")]
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private bool _networkedOnly;

        private void Awake()
        {
            if (_networkManager == null)
                _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        }
    
        public override bool CanInteract()
        {
            return !_networkedOnly || _networkManager.IsOnline;
        }
    }
}
