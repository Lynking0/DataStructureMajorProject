using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    public partial class RTree
    {
        private abstract class RTNode : IShape
        {
            public RTree RTree;
            public RTNode Parent;
            /// <summary>
            ///   当前节点的MBR
            /// </summary>
            public RTRect2 Rectangle { get; private set; }
            /// <summary>
            ///   RTNode和TData均实现IShape，为RTIntlNode和RTLeafNode获取子MBR的统一方法
            ///   选择IEnumerable<T>仅因为IEnumerable<T>支持协变
            /// </summary>
            public abstract IEnumerable<IShape> SubShapes { get; }
            public abstract int SubShapesCount { get; }
            protected RTNode(RTree rTree, RTNode parent)
            {
                RTree = rTree;
                Parent = parent;
            }
            /// <summary>
            ///   搜索子树中所有MBR与搜索框有重叠的条目
            /// </summary>
            public void SearchLeaves(RTRect2 searchArea, List<IRTreeData> result)
            {
                foreach (IShape shape in SubShapes)
                {
                    if (searchArea.IsOverLap(shape.Rectangle))
                    {
                        if (shape is RTNode child)
                            child.SearchLeaves(searchArea, result);
                        else if (shape is IRTreeData data)
                            result.Add(data);
                    }
                }
            }
            /// <summary>
            ///   若SubShapes数量超出上限，则进行分裂
            /// </summary>
            /// <returns>是否进行了分裂操作</returns>
            public bool TrySplit()
            {
                if (SubShapesCount <= RTree.Capacity)
                    return false;
                return true;
            }
        }
    }
}