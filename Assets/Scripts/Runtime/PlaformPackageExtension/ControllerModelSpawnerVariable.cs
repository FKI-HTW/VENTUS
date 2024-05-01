using CENTIS.XRPlatformManagement.Controller.Manager;
using UnityEngine;
using VENTUS.DataStructures.Variables;

namespace VENTUS.PlaformPackageExtension
{
	[CreateAssetMenu(fileName = "new ControllerModelRendererVariable", menuName = "VENTUS/PlatformPackageExtension/Controller Model Renderer Variable")]
	public class ControllerModelSpawnerVariable : AbstractVariable<ControllerModelSpawner>, IScriptableObjectRegister
    {
        public void Register(GameObject relatedGameObject)
        {
            Set(relatedGameObject.GetComponent<ControllerModelSpawner>());
        }

        public void Unregister()
        {
            Restore();
        }
    }
}
