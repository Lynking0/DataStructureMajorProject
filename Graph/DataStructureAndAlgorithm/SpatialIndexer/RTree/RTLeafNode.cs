using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    public partial class RTree<TData>
    {
        private class RTLeafNode : RTNode
        {
            public List<TData> Datas;
            public override IEnumerable<IShape> SubShapes { get => Datas; }
            public RTLeafNode(RTree<TData> rTree, RTNode parent) : base(rTree, parent)
            {
                Datas = new List<TData>();
            }
        }
    }
}