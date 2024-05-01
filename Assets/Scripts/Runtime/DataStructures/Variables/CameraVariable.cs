using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
	[CreateAssetMenu(fileName = "new CameraVariable", menuName = "VENTUS/DataStructures/Variables/Camera")]
	public class CameraVariable : AbstractVariable<Camera>, IScriptableObjectRegister
	{
		public void Register(GameObject relatedGameObject)
		{
			Set(relatedGameObject.GetComponent<Camera>());
		}

		public void Unregister()
		{
			Restore();
		}
	}
}
