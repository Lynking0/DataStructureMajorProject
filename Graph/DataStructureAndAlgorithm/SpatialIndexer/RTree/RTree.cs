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
        private int MinimumCapacity;
        /// <summary>
        ///   根节点
        /// </summary>
        private RTNode Root;
        public int Count { get; private set; }
        public bool IsReadOnly { get => false; }
        public RTree(int capacity)
        {
            Root = new RTLeafNode(this, null, 0);
            Count = 0;
            Capacity = capacity;
            MinimumCapacity = Godot.Mathf.FloorToInt(capacity / 2);
        }
        /// <summary>
        ///   选择叶子结点以放置新条目
        /// </summary>
        private RTLeafNode ChooseLeaf(RTRect2 targetMBR)
        {
            RTNode thisNode = Root;
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
        /// <summary>
        ///   选择指定层级的中间结点以放置新条目
        /// </summary>
        private RTIntlNode ChooseIntlNode(RTRect2 targetMBR, int level)
        {
            RTIntlNode node = (Root as RTIntlNode)!;
            while (true)
            {
                if (node.Level == level)
                    return node;
                RTNode bestChild = node.Children[0];
                double minCost = node.Children[0].Rectangle.CalcExpandCost(targetMBR);
                for (int i = 1; i < node.Children.Count; ++i)
                {
                    double cost = node.Children[i].Rectangle.CalcExpandCost(targetMBR);
                    if (cost < minCost)
                    {
                        bestChild = node.Children[i];
                        minCost = cost;
                    }
                }
                node = (bestChild as RTIntlNode)!;
            }
        }
        /// <summary>
        ///   Add后对R树进行调整
        /// </summary>
        private void CascadeAdjust(RTNode start)
        {
            for (RTNode? node = start; node is not null; node = node.Parent)
                node.Adjust();
        }
        public bool CanAdd(IRTreeData data)
        {
            List<IRTreeData> adjacencies = new List<IRTreeData>();
            Root.SearchLeaves(data.Rectangle, adjacencies);
            foreach (IRTreeData other in adjacencies)
                if (data.IsOverlap(other))
                    return false;
            return true;
        }
        /// <summary>
        ///   获取搜索矩阵附近的边
        /// </summary>
        public IEnumerable<Edge> Search(RTRect2 rect)
        {
            List<IRTreeData> adjacencies = new List<IRTreeData>();
            Root.SearchLeaves(rect, adjacencies);
            foreach (IRTreeData data in adjacencies)
                if (data is Edge edge)
                    yield return edge;
        }
        /// <summary>
        ///   添加一个data，此函数不会判断添加操作是否合法，调用此函数前需确保已调用CanAdd检测合法性。
        /// </summary>
        public void Add(IRTreeData data)
        {
            RTLeafNode leaf = ChooseLeaf(data.Rectangle);
            leaf.Datas.Add(data);
            ++Count;
            CascadeAdjust(leaf);
        }
        /// <summary>
        ///   将一个RTNode及其子树添加进R树
        /// </summary>
        private void AddNode(RTNode node)
        {
            node.Parent = ChooseIntlNode(node.Rectangle, node.Level + 1);
            (node.Parent as RTIntlNode)!.Children.Add(node);
            CascadeAdjust(node.Parent);
        }
        /// <summary>
        ///   Remove后对R树进行缩减
        /// </summary>
        private void CascadeCondense(RTNode start)
        {
            List<IShape> RemovedShapes = new List<IShape>();
            for (RTNode? node = start; node is not null; node = node.Parent)
                node.Condense(RemovedShapes);
            // 将刚刚临时删除的条目重新添加
            foreach (IShape shape in RemovedShapes)
            {
                if (shape is IRTreeData data)
                    Add(data);
                if (shape is RTNode node)
                    AddNode(node);
            }
        }
        public bool Remove(IRTreeData data)
        {
            if (Root.FindLeaf(data) is not RTLeafNode leaf)
                return false;
            leaf.Datas.Remove(data);
            --Count;
            CascadeCondense(leaf);
            return true;
        }
        public void Clear()
        {
            Root = new RTLeafNode(this, null, 0);
            Count = 0;
        }
        public bool Contains(IRTreeData data)
        {
            return Root.FindLeaf(data) is not null;
        }
        public void CopyTo(IRTreeData[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            throw new NotImplementedException();
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
        /// <summary>
        ///   按照深度顺序遍历R树各个节点的MBR
        /// </summary>
        public IEnumerable<(int depth, RTRect2 rect)> RectangleTraversal()
        {
            if (Count != 0)
            {
                Queue<(int, RTNode)> queue = new Queue<(int, RTNode)>();
                queue.Enqueue((0, Root));
                yield return (0, Root.Rectangle);
                while (queue.Count != 0)
                {
                    (int depth, RTNode node) = queue.Dequeue();
                    foreach (IShape shape in node.SubShapes)
                    {
                        if (shape is RTNode nextNode)
                            queue.Enqueue((depth + 1, nextNode));
                        yield return (depth + 1, shape.Rectangle);
                    }
                }
            }
        }
    }
}