using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    public partial class RTree<TData>
    {
        private abstract class RTNode : IShape
        {
            public RTree<TData> RTree;
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
            protected RTNode(RTree<TData> rTree, RTNode parent)
            {
                RTree = rTree;
                Parent = parent;
            }
        }
    }
}