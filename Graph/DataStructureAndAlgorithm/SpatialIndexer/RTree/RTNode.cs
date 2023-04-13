using System;
using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    public partial class RTree
    {
        private abstract class RTNode : IShape
        {
            public RTree RTree;
            public RTNode? Parent;
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
            protected RTNode(RTree rTree, RTNode? parent)
            {
                RTree = rTree;
                Parent = parent;
            }
            /// <summary>
            ///   添加shape，不改变MBR
            /// </summary>
            protected abstract void AddShape(IShape shape);
            /// <summary>
            ///   清空shape并分裂，不改变MBR
            /// </summary>
            protected abstract RTNode ClearAndSplit();
            protected abstract IShape[] GetSubShapesCopy();
            /// <summary>
            ///   检查Parent是否为空，若为空，则创建一个Parent并调整相关数据（不更改MBR）
            /// </summary>
            protected void CheckParent()
            {
                if (Parent is null)
                {
                    RTIntlNode parent = new RTIntlNode(RTree, null);
                    parent.Children.Add(this);
                    RTree.Root = Parent = parent;
                }
            }
            /// <summary>
            ///   更新MBR
            /// </summary>
            protected void UpdateMBR()
            {
                double minX = double.PositiveInfinity;
                double minY = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double maxY = double.NegativeInfinity;
                foreach (IShape shape in SubShapes)
                {
                    if (shape.Rectangle.TL.X < minX)
                        minX = shape.Rectangle.TL.X;
                    if (shape.Rectangle.TL.Y < minY)
                        minY = shape.Rectangle.TL.Y;
                    if (shape.Rectangle.BR.X > maxX)
                        maxX = shape.Rectangle.BR.X;
                    if (shape.Rectangle.BR.Y > maxY)
                        maxY = shape.Rectangle.BR.Y;
                }
                Rectangle = new RTRect2(
                    new Vector2D(minX, minY),
                    new Vector2D(maxX, maxY)
                );
            }
            /// <summary>
            ///   搜索子树中所有MBR与搜索框有重叠的条目
            /// </summary>
            public abstract void SearchLeaves(RTRect2 searchArea, List<IRTreeData> result);
            /// <summary>
            ///   更新节点的MBR，
            ///   若SubShapes数量超出上限，则先进行分裂再更新
            /// </summary>
            /// <returns>是否进行了分裂操作</returns>
            public void Adjust()
            {
                if (SubShapesCount <= RTree.Capacity)
                {
                    UpdateMBR();
                    return;
                }
                double maxIncrement = double.NegativeInfinity;
                IShape? seed1 = null, seed2 = null;
                foreach ((IShape a, IShape b) in SubShapes.ToPairs())
                {
                    double temp = RTRect2.CalcExpandCost(a.Rectangle, b.Rectangle);
                    if (temp > maxIncrement)
                    {
                        seed1 = a;
                        seed2 = b;
                        maxIncrement = temp;
                    }
                }
                IShape[] subShapesCopy = GetSubShapesCopy();
                RTNode splitNode = ClearAndSplit();
                foreach (IShape shape in subShapesCopy)
                {
                    if (shape == seed1)
                    {
                        this.AddShape(shape);
                        continue;
                    }
                    if (shape == seed2)
                    {
                        splitNode.AddShape(shape);
                        continue;
                    }
                    double dist1 = RTRect2.CalcExpandCost(shape.Rectangle, seed1!.Rectangle);
                    double dist2 = RTRect2.CalcExpandCost(shape.Rectangle, seed2!.Rectangle);
                    if (dist1 < dist2)
                        this.AddShape(shape);
                    else
                        splitNode.AddShape(shape);
                }
                // 调整MBR
                this.UpdateMBR();
                splitNode.UpdateMBR();
            }
        }
    }
}