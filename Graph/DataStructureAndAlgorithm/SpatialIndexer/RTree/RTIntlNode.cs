using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    public partial class RTree
    {
        private class RTIntlNode : RTNode
        {
            public List<RTNode> Children;
            public override IEnumerable<IShape> SubShapes { get => Children; }
            public RTIntlNode(RTree rTree, RTNode parent) : base(rTree, parent)
            {
                Children = new List<RTNode>();
            }
        }
    }
}