using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    /// <summary>
    ///   二维空间上的R树
    /// </summary>
    public partial class RTree : ICollection<IRTreeData>
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
            MininumCapacity = Mathf.CeilToInt(Capacity / 2.0);
        }
        /// <summary>
        ///   选择叶子结点以放置新条目
        /// </summary>
        private RTLeafNode ChooseLeaf(RTRect2 targetMBR)
        {
            RTNode thisNode = Root!;
            while (true)
            {
                if (thisNode is RTLeafNode leafNode)
                    return leafNode;
                if (thisNode is RTIntlNode intlNode)
                {
                    RTNode bestChild = intlNode.Children[0];
                    double minCost = intlNode.Children[0].Rectangle.CalcExpandCost(targetMBR);
                    for (int i = 1; i < intlNode.Children.Count; ++i)
                    {
                        double cost = intlNode.Children[i].Rectangle.CalcExpandCost(targetMBR);
                        if (cost < minCost)
                        {
                            bestChild = intlNode.Children[i];
                            minCost = cost;
                        }
                    }
                    thisNode = bestChild;
                }
            }
        }
        public bool CanAdd(IRTreeData data)
        {
            if (Root is null)
                return true;
            List<IRTreeData> adjacencies = new List<IRTreeData>();
            Root.SearchLeaves(data.Rectangle, adjacencies);
            foreach (IRTreeData other in adjacencies)
                if (data.IsOverlap(other))
                    return false;
            return true;
        }
        public void Add(IRTreeData data)
        {
            
        }
        [Obsolete("删除功能还没写！")]
        public bool Remove(IRTreeData data)
        {
            return true;
        }
        public void Clear()
        {
            Root = null;
            Count = 0;
        }
        public bool Contains(IRTreeData data)
        {
            return true;
        }
        public void CopyTo(IRTreeData[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

        }
        private IEnumerator<IRTreeData> _getEnumerator(RTNode? thisNode)
        {
            if (thisNode is RTIntlNode intlNode)
            {
                foreach (RTNode child in intlNode.Children)
                    for (IEnumerator<IRTreeData> it = _getEnumerator(child); it.MoveNext();)
                        yield return it.Current;
            }
            else if (thisNode is RTLeafNode leafNode)
            {
                foreach (IRTreeData data in leafNode.Datas)
                    yield return data;
            }
        }
        public IEnumerator<IRTreeData> GetEnumerator()
        {
            return _getEnumerator(Root);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}