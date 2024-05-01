using System.Collections.Generic;
using UnityEngine;

namespace VENTUS.DataStructures.RuntimeSet
{
    public class RuntimeDictionary<Key, Value> : ScriptableObject
    {
        [SerializeField] protected readonly Dictionary<Key, Value> items = new Dictionary<Key, Value>();

        public Dictionary<Key, Value> GetItems()
        {
            return items;
        }

        public bool TryGetContent(Key key, out Value value)
        {
            return items.TryGetValue(key, out value);
        }

        public Value GetContent(Key key)
        {
            return items[key];
        }

        public void Add(Key key, Value value)
        {
            if (!items.ContainsKey(key))
            {
                items.Add(key, value);
            }
        }

        public void Remove(Key key)
        {
            if (items.ContainsKey(key))
            {
                items.Remove(key);
            }
        }

        public void Restore()
        {
            items.Clear();
        }
    }
}
