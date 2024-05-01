using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
    [CreateAssetMenu(fileName = "TransformVariable", menuName = "VENTUS/DataStructures/Variables/Transform")]
    public class TransformVariable : AbstractVariable<Transform>, IScriptableObjectRegister
    {
        public void Register(GameObject relatedGameObject)
        {
            Set(relatedGameObject.transform);
        }

        public void Unregister()
        {
            Restore();
        }
    }
}
