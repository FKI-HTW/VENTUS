using System.Collections.Generic;
using System.Linq;

namespace VENTUS.DataStructures
{
	/** 
     * by https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Collections/SerializableDictionaryBase.cs
     * 
     * Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
     * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
     * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
     * 
     * SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
     * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */

	public abstract class DrawableDictionary
    {

    }

    [System.Serializable()]
    public class SerializableDictionary<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {

        #region Fields

        [System.NonSerialized()]
        private Dictionary<TKey, TValue> _dict;
        [System.NonSerialized()]
        private readonly IEqualityComparer<TKey> _comparer;

        #endregion

        #region Constructor

        public SerializableDictionary()
        {

        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        #endregion

        #region Properties

        public IEqualityComparer<TKey> Comparer
        {
            get { return _comparer; }
        }

        #endregion

        #region IDictionary Interface

        public int Count
        {
            get { return (_dict != null) ? _dict.Count : 0; }
        }

        public void Add(TKey key, TValue value)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (_dict == null) return false;
            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict == null) return false;
            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict == null)
            {
                value = default(TValue);
                return false;
            }
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dict == null) throw new KeyNotFoundException();
                return _dict[key];
            }
            set
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                _dict[key] = value;
            }
        }

        public void Clear()
        {
            if (_dict != null) _dict.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dict == null) return;
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (_dict == null) return default(Dictionary<TKey, TValue>.Enumerator);
            return _dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_dict == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_dict == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        public Dictionary<TKey, TValue> GetDictionary() => _dict;


        #endregion

        #region ISerializationCallbackReceiver

        [UnityEngine.SerializeField()]
        private TKey[] _keys;
        [UnityEngine.SerializeField()]
        private TValue[] _values;

        void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keys != null && _values != null)
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_keys.Length, _comparer);
                else _dict.Clear();
                for (int i = 0; i < _keys.Length; i++)
                {
                    if (i < _values.Length)
                        _dict[_keys[i]] = _values[i];
                    else
                        _dict[_keys[i]] = default(TValue);
                }
            }

            _keys = null;
            _values = null;
        }

        void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dict == null || _dict.Count == 0)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                int cnt = _dict.Count;
                _keys = new TKey[cnt];
                _values = new TValue[cnt];
                int i = 0;
                var e = _dict.GetEnumerator();
                while (e.MoveNext())
                {
                    _keys[i] = e.Current.Key;
                    _values[i] = e.Current.Value;
                    i++;
                }
            }
        }

        #endregion

    }
}
