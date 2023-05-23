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

        public GroupIndexedList(IEnumerable<T> data, Func<T, T_K1> key1_extractor)
        {
            _key1_extractor = key1_extractor;
            AddRange(data);
        }

        public List<T> this[T_K1 k] => _key1_group[k];

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

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }
    }
}