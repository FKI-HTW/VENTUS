using System;
using UnityEngine;

namespace VENTUS.DataStructures.Variables
{
    [Serializable]
    public abstract class AbstractReference<T>
    {
        [SerializeField] private bool useOverride;
        [SerializeField] private T overrideValue;
        [SerializeField] private AbstractVariable<T> variable;

        protected AbstractReference(T value)
        {
            useOverride = true;
            overrideValue = value;
        }

        public void SetOverride(T value)
        {
            useOverride = true;
            overrideValue = value;
        }

        public void ResetOverride()
        {
            useOverride = false;
        }
        
        protected T value => useOverride ? overrideValue : variable.Get();

        public static implicit operator T(AbstractReference<T> reference)
        {
            return reference.value;
        }
    }
}