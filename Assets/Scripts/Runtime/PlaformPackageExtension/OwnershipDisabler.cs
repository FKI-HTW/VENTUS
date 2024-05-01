using FishNet.Object;
using UnityEngine;

namespace VENTUS.PlaformPackageExtension
{
    [RequireComponent(typeof(NetworkObject))]
    public class OwnershipDisabler : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            for (int index = 0; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(!IsOwner);
            }
        }
    }
}
