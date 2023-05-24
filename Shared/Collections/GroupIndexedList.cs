using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Collections
{
    public class GroupIndexedList<T, T_K1> : ICollection<T> where T_K1 : notnull
    {
        private List<T> _list = new List<T>();
        private Dictionary<T_K1, List<T>> _key1_group = new Dictionary<T_K1, List<T>>();
        private Func<T, T_K1> _key1_extractor;

        public GroupIndexedList(Func<T, T_K1> key1_extractor)
        {
            _key1_extractor = key1_extractor;
        }
        public GroupIndexedList(IEnumerable<T> data, Func<T, T_K1> key1_extractor)
        {
            _key1_extractor = key1_extractor;
            AddRange(data);
        }

        public IReadOnlyList<T> this[T_K1 k] => _key1_group[k];

        public int Count => _list.Count;

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(item);
            var key1 = _key1_extractor(item);
            if (!_key1_group.ContainsKey(key1))
            {
                _key1_group.Add(key1, new List<T>());
            }
            _key1_group[key1].Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _key1_group.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }
        
        public bool ContainsKey(T_K1 key)
        {
            return _key1_group.ContainsKey(key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(T item)
        {
            if (!_list.Remove(item))
                return false;
            if (!_key1_group[_key1_extractor(item)].Remove(item))
                return false;
            return true;
        }
    }

    public class GroupIndexedList<T, T_K1, T_K2> : ICollection<T> where T_K1 : notnull where T_K2 : notnull
    {
        private List<T> _list = new List<T>();
        private Dictionary<T_K1, List<T>> _key1_group = new Dictionary<T_K1, List<T>>();
        private Dictionary<T_K2, List<T>> _key2_group = new Dictionary<T_K2, List<T>>();
        private Func<T, T_K1> _key1_extractor;
        private Func<T, T_K2> _key2_extractor;

        public GroupIndexedList(Func<T, T_K1> key1_extractor, Func<T, T_K2> key2_extractor)
        {
            _key1_extractor = key1_extractor;
            _key2_extractor = key2_extractor;
        }
        public GroupIndexedList(IEnumerable<T> data, Func<T, T_K1> key1_extractor, Func<T, T_K2> key2_extractor)
        {
            _key1_extractor = key1_extractor;
            _key2_extractor = key2_extractor;
            AddRange(data);
        }

        public IReadOnlyList<T> this[T_K1 k] => _key1_group[k];
        public IReadOnlyList<T> this[T_K2 k] => _key2_group[k];

        public int Count => _list.Count;

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(item);
            var key1 = _key1_extractor(item);
            if (!_key1_group.ContainsKey(key1))
            {
                _key1_group.Add(key1, new List<T>());
            }
            _key1_group[key1].Add(item);

            var key2 = _key2_extractor(item);
            if (!_key2_group.ContainsKey(key2))
            {
                _key2_group.Add(key2, new List<T>());
            }
            _key2_group[key2].Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _key1_group.Clear();
            _key2_group.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public bool ContainsKey(T_K1 key)
        {
            return _key1_group.ContainsKey(key);
        }

        public bool ContainsKey(T_K2 key)
        {
            return _key2_group.ContainsKey(key);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(T item)
        {
            if (_key1_group[_key1_extractor(item)].Remove(item) && _key2_group[_key2_extractor(item)].Remove(item))
            {
                return _list.Remove(item);
            }
            return false;
        }
    }
}