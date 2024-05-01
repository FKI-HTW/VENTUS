using System;
using System.Collections.Generic;

namespace VENTUS.DataStructures.ServiceProvider
{
    public static class GenericSerciveProvider
    {
        private static readonly Dictionary<Type, object> _lookupTable = new();

        public static void AddService(object service)
        {
            Type key = service.GetType();
            _lookupTable.TryAdd(key, service);
		}

        public static void RemoveService(object service)
        {
            Type key = service.GetType();
            _lookupTable.Remove(key);
		}

        public static bool TryGetService<T>(out T service)
        {
            service = default;
            Type key = typeof(T);

            if (!_lookupTable.TryGetValue(key, out object value)) return false;
        
            if (value is T castedType) {
                service = castedType;
                return true;
            } 
        
            try {
                service = (T)Convert.ChangeType(value, typeof(T));
                return true;
            } 
            catch (InvalidCastException) {
                return false;
            }
        }
    }
}