using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
    [CreateAssetMenu(fileName = "new Vector3Variable", menuName = "VENTUS/DataStructures/Variables/Vector3")]
    public class Vector3Variable : AbstractVariable<Vector3>
    {
        public void Add(Vector3 value)
        {
            runtimeValue += value;
            if (onValueChanged != null) onValueChanged.Raise();
        }

        public void Add(Vector3Variable value)
        {
            runtimeValue += value.runtimeValue;
            if (onValueChanged != null) onValueChanged.Raise();
        }
    }
}
