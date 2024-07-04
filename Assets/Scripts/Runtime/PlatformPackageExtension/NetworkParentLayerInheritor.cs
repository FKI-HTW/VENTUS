using FishNet.Object;
using Unity.XR.CoreUtils;

namespace VENTUS.PlatformPackageExtension
{
    public class NetworkParentLayerInheritor : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
        
            gameObject.SetLayerRecursively(transform.parent.gameObject.layer);
        }
    }
}
