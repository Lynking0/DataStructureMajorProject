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
            public override int SubShapesCount { get => Datas.Count; }
            public RTLeafNode(RTree rTree, RTNode? parent) : base(rTree, parent)
            {
                Datas = new List<IRTreeData>();
            }
            protected override void AddShape(IShape shape)
            {
                if (shape is IRTreeData data)
                    Datas.Add(data);
                else
                    throw new Exception($"{GetType()}.AddShape(IShape): Unexpected type <{shape.GetType()}>.");
            }
            protected override RTNode ClearAndSplit()
            {
                CheckParent();
                RTLeafNode newNode = new RTLeafNode(RTree, Parent);
                (Parent as RTIntlNode)!.Children.Add(newNode);
                Datas = new List<IRTreeData>();
                return newNode;
            }
            protected override IShape[] GetSubShapesCopy()
            {
                return Datas.ToArray();
            }
            public override void SearchLeaves(RTRect2 searchArea, List<IRTreeData> result)
            {
                foreach (IRTreeData data in Datas)
                    if (searchArea.IsOverLap(data.Rectangle))
                        result.Add(data);
            }
        }
    }
}