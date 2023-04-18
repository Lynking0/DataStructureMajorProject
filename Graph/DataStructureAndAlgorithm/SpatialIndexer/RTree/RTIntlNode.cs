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
            public RTIntlNode(RTree rTree, RTNode? parent, int level) : base(rTree, parent, level)
            {
                Children = new List<RTNode>();
            }
            protected override void AddShape(IShape shape)
            {
                if (shape is RTNode child)
                {
                    Children.Add(child);
                    child.Parent = this;
                }
                else
                    throw new Exception($"{GetType()}.AddShape(IShape): Unexpected type <{shape.GetType()}>.");
            }
            protected override RTNode ClearAndSplit()
            {
                CheckParent();
                RTIntlNode newNode = new RTIntlNode(RTree, Parent, Level);
                (Parent as RTIntlNode)!.Children.Add(newNode);
                Children = new List<RTNode>();
                return newNode;
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
            public override RTLeafNode? FindLeaf(IRTreeData data)
            {
                foreach (RTNode child in Children)
                    if (data.Rectangle.IsOverLap(child.Rectangle))
                        if (child.FindLeaf(data) is RTLeafNode leaf)
                            return leaf;
                return null;
            }
        }
    }
}