using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
    [CreateAssetMenu(fileName = "new FloatVariable", menuName = "VENTUS/DataStructures/Variables/Float")]
    public class FloatVariable : AbstractVariable<float>
    {
        public void Add(float value)
        {
            runtimeValue += value;
            if (onValueChanged != null) onValueChanged.Raise();
        }

        public void Add(FloatVariable value)
        {
            runtimeValue += value.runtimeValue;
            if (onValueChanged != null) onValueChanged.Raise();
        }
    }
}
