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
            public override int SubShapesCount { get => Children.Count; }
            public RTIntlNode(RTree rTree, RTNode? parent) : base(rTree, parent)
            {
                Children = new List<RTNode>();
            }
            protected override void AddShape(IShape shape)
            {
                if (shape is RTNode child)
                    Children.Add(child);
                throw new Exception($"{GetType()}.AddShape(IShape): Unexpected type.");
            }
            protected override RTNode ClearAndSplit()
            {
                Children = new List<RTNode>();
                return new RTIntlNode(RTree, Parent);
            }
            protected override IShape[] GetSubShapesCopy()
            {
                return Children.ToArray();
            }
            public override void SearchLeaves(RTRect2 searchArea, List<IRTreeData> result)
            {
                foreach (RTNode child in Children)
                    if (searchArea.IsOverLap(child.Rectangle))
                            child.SearchLeaves(searchArea, result);
            }
        }
    }
}