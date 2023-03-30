using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    public partial class RTree<TData>
    {
        private class RTIntlNode : RTNode
        {
            public List<RTNode> Children;
            public override IEnumerable<IShape> SubShapes { get => Children; }
            public RTIntlNode(RTree<TData> rTree, RTNode parent) : base(rTree, parent)
            {
                Children = new List<RTNode>();
            }
        }
    }
}