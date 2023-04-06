using System;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    public partial class RTree
    {
        private class RTLeafNode : RTNode
        {
            public List<IRTreeData> Datas;
            public override IEnumerable<IShape> SubShapes { get => Datas; }
            public RTLeafNode(RTree rTree, RTNode parent) : base(rTree, parent)
            {
                Datas = new List<IRTreeData>();
            }
        }
    }
}