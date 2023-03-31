using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    /// <summary>
    ///   二维空间上的R树
    /// </summary>
    /// <typeparam name="TData">储存的数据类型</typeparam>
    public partial class RTree<TData> : ICollection<TData> where TData : class, IRTreeData<TData>
    {
        /// <summary>
        ///   中间节点的最大子节点数以及叶子节点最大记录索引数
        /// </summary>
        private int Capacity;
        /// <summary>
        ///   中间节点的最小子节点数以及叶子节点最小记录索引数（根节点除外）
        /// </summary>
        private int MininumCapacity;
        /// <summary>
        ///   根节点
        /// </summary>
        private RTNode? Root;
        public int Count { get; private set; }
        public bool IsReadOnly { get => false; }
        public RTree(int capacity)
        {
            Root = null;
            Count = 0;
            Capacity = capacity;
        }
        public List<TData> Search(RTRect2 searchArea)
        {

        }
        public void Add(TData data)
        {
        }
        public bool Remove(TData data)
        {
            return true;
        }
        public void Clear()
        {
            Root = null;
            Count = 0;
        }
        public bool Contains(TData data)
        {
            return true;
        }
        public void CopyTo(TData[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

        }
        private IEnumerator<TData> _getEnumerator(RTNode? thisNode)
        {
            if (thisNode is RTIntlNode intlNode)
            {
                foreach (RTNode child in intlNode.Children)
                    for (IEnumerator<TData> it = _getEnumerator(child); it.MoveNext();)
                        yield return it.Current;
            }
            else if (thisNode is RTLeafNode leafNode)
            {
                foreach (TData data in leafNode.Datas)
                    yield return data;
            }
        }
        public IEnumerator<TData> GetEnumerator()
        {
            return _getEnumerator(Root);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}