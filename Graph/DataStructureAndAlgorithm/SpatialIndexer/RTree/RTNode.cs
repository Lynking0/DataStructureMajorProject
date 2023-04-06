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
            protected RTNode(RTree rTree, RTNode parent)
            {
                RTree = rTree;
                Parent = parent;
            }
        }
    }
}